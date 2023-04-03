<?php
declare(strict_types=1);

namespace App\Controller\Checker;

use App\Controller\AppController;
use App\Service\PDFService;
use App\Service\SendMailService;
use Cake\Core\Configure;
use Cake\Datasource\ConnectionManager;

/**
 * FarmApplication Controller
 *
 * @property \App\Model\Table\UsersTable $Users
 * @property \App\Model\Table\FarmQuestionsTable $FarmQuestions
 * @property \App\Model\Table\OfficesTable $Offices
 * @method \App\Model\Entity\FarmApplication[]|\Cake\Datasource\ResultSetInterface paginate($object = null, array $settings = [])
 */
class FarmApplicationController extends AppController
{
    public $paginate = [
        'limit' => PAGINATE_DEFAULT,
        'order' => [
            'FarmQuestions.id' => 'asc'
        ]
    ];

    private PDFService $pdfService;

    private SendMailService $sendMailService;

    public function beforeFilter(\Cake\Event\EventInterface $event)
    {
        parent::beforeFilter($event);

        $this->Authentication->addUnauthenticatedActions([]);
        $this->viewBuilder()->setLayout("user/checker_layout");
    }

    /**
     * Initialization hook method.
     *
     * @return void
     */
    public function initialize(): void
    {
        parent::initialize();
        $this->loadModel('FarmQuestions');
        $this->loadModel('Offices');
        $this->pdfService = new PDFService();
        $this->sendMailService = new SendMailService();
        $this->loadComponent('Flash'); // Include the FlashComponent
    }

    /**
     * Farms answers list
     *
     * @return \Cake\Http\Response|null|void Renders view
     */
    public function list(): void
    {
        $farmQuestions = $this->FarmQuestions->getListAnswer($this->FarmQuestions, ['fields' => ['id', 'modified_at', 'status']]);
        $this->set('farmQuestions',  $this->paginate($farmQuestions));
    }

    /**
     * Show the score of the farm application
     *
     * @param int $id FarmQuestion id.
     * @return \Cake\Http\Response|null|void Redirects on successful save, renders view otherwise.
     * @throws \Cake\Datasource\Exception\RecordNotFoundException When record not found.
     */
    public function score(int $id)
    {
        $farmAnswer = $this->FarmQuestions->getAnwserWithRelationships($this->FarmQuestions, $id, ['Offices.Businesses']);
        Configure::load('farm_questions', 'default');
        $farmQuestions = Configure::read('questions');

        if ($this->request->is(['patch', 'post', 'put'])) {
            $scoreData = array_diff_key($this->request->getData(), ['tab_item' => 'on']);
            $data = array_merge(
                    array_map(function($item) {
                        return json_encode(array_map('intval', $item), JSON_FORCE_OBJECT);
                    }, $scoreData),
                    ['status' => APPLICATION_STATUS_SCORED]
                );

            $farmAnswer = $this->FarmQuestions->patchEntity($farmAnswer, $data);

            if (!$farmAnswer->getErrors()) {
                if ($this->FarmQuestions->save($farmAnswer)) {
                    $this->Flash->success(SCORE_FARM_ANSWER_SUCCESS);
                    return $this->redirect(['action' => 'scoreTotal', 'id' => $id]);
                }
            }
        }

        $this->set(['farmQuestions', 'farmAnswer'], [$farmQuestions, $farmAnswer]);
    }

    /**
     * Show the total score of the farm application
     *
     * @param int $id FarmQuestion id.
     * @return \Cake\Http\Response|null|void Renders view
     * @throws \Cake\Datasource\Exception\RecordNotFoundException When record not found.
     */
    public function scoreTotal(int $id)
    {
        $farmAnswer = $this->FarmQuestions->getAnwserWithRelationships($this->FarmQuestions, $id, ['Offices.Businesses']);
        Configure::load('farm_questions', 'default');
        $farmQuestions = Configure::read('questions');
        $maxScoreByGroup = Configure::read('max_score_group');

        $totalScore = array_reduce(range(1, count($maxScoreByGroup)), function($totalScore, $i) use ($farmAnswer, $maxScoreByGroup) {
            $totalScoreGroup = array_reduce($maxScoreByGroup["group{$i}"]['questions'], function ($total, $idx) use ($farmAnswer) {
                $score = json_decode($farmAnswer->{"question{$idx}_score"}, true);

                return $total += array_sum(array_values($score));
            }, 0);
            $totalScoreGroup = min($totalScoreGroup, (int) $maxScoreByGroup["group{$i}"]['max_score']);

            return $totalScore += $totalScoreGroup;
        }, 0);

        if ($this->request->is(['patch', 'post', 'put'])) {
            $farmAnswer = $this->FarmQuestions->patchEntity($farmAnswer, ['total_score' => $totalScore]);

            if (!$farmAnswer->getErrors() && $this->FarmQuestions->save($farmAnswer)) {
                return $this->redirect(['action' => 'sendMail', 'id' => $id]);
            }
        }

        $this->set(['farmQuestions', 'farmAnswer', 'total'], [$farmQuestions, $farmAnswer, $totalScore]);
    }

    /**
     * Checker send mail to farm application
     *
     * @return \Cake\Http\Response|null|void Renders view
     */
    public function sendMail(int $id)
    {
        $farmQuestion = $this->FarmQuestions->getAnwserWithRelationships($this->FarmQuestions, $id, [
            'Offices.Businesses' => [
                'fields' => [
                    'Offices.name',
                    'Offices.office_type',
                    'Offices.offices_certified_rank',
                    'Businesses.name',
                    'Businesses.representative_name',
                    'Businesses.prefecture',
                    'Businesses.city',
                    'Businesses.street',
                    'Businesses.email'
                ]
            ]
        ]);

        $office = $this->Offices->findById($farmQuestion->office_id)->first();

        if($this->request->is(['post'])) {
            $rank = (int) $this->request->getData('offices_certified_rank');
            $office->offices_certified_rank = $rank;
            $office = $this->Offices->patchEntity($office,
                array_merge(
                    $office->toArray(),
                    ['offices_certified_rank' => $rank]
                )
            );

            if ($office->getErrors()) {
                $this->Flash->error(array_values($office->getErrors())[0]['validValue']);
            } else {
                $certificatePath = null;
                $data = array_merge(
                    $office->certificate,
                    [
                        'office_type'           => $farmQuestion->office->office_type,
                        'office_name'           => $farmQuestion->office->name,
                        'business_name'         => $farmQuestion->office->business->name,
                        'representative_name'   => $farmQuestion->office->business->representative_name,
                        'total_score'           => $farmQuestion->total_score,
                        'business_address'      => h($farmQuestion->office->business->prefecture)
                                                    . h($farmQuestion->office->business->city)
                                                    . h($farmQuestion->office->business->street),
                        'certification_date'    => date("Y年m月d日")
                    ]
                );

                if (in_array($rank, APPROVE_RANK_LIST)) {
                    $certificatePath      = $this->pdfService->convertHtmlToPdf($data);
                    $farmQuestion->status = APPLICATION_STATUS_APPROVED;
                } else {
                    $farmQuestion->status = APPLICATION_STATUS_REJECTED;
                }

                if ($farmQuestion->status === APPLICATION_STATUS_APPROVED && !$certificatePath) {
                    $this->Flash->error(ERROR_OCCUR);
                    $this->redirect(['action' => 'list']);
                } else {
                    $office->offices_certified_file_path = $certificatePath;
                    $farmQuestion->office_id = $office->id;

                    $farmQuestion = $this->FarmQuestions->patchEntity($farmQuestion, $this->request->getData());
                    if (!$farmQuestion->getErrors()) {
                        $conn = ConnectionManager::get('default');
                        $conn->begin();

                        try {
                            $conn->transactional(function ($conn) use ($farmQuestion, $office, $data) {
                                if ($this->Offices->save($office)) {
                                    $this->FarmQuestions->save($farmQuestion);

                                    $args = $farmQuestion;
                                    $args->certificate_path = $office->offices_certified_file_path;
                                    $args->certificate_rank = $office->offices_certified_rank;
                                    $args->certificate_rank_label = $data['rank_label'];
                                    $args->mark_path = $data['mark_image'];
                                    $this->sendMailService->certificationNotify($args);

                                    $conn->commit();
                                    $this->Flash->success(SEND_MAIL_SUCCESSFULLY);
                                    $this->redirect(['action' => 'list']);
                                }
                            });
                        } catch (\Cake\ORM\Exception\PersistenceFailedException $e) {
                            $conn->rollback();
                        }
                    }
                }
            }
        }

        $this->set('farmQuestion', $farmQuestion);
    }
}
