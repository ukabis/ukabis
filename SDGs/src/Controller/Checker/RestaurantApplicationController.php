<?php

declare(strict_types=1);

namespace App\Controller\Checker;

use App\Controller\AppController;
use App\Service\PDFService;
use App\Service\SendMailService;
use Cake\Datasource\ConnectionManager;
use Cake\Http\Exception\NotFoundException;

/**
 * RestaurantApplicationController Controller
 * @property \App\Model\Table\UsersTable $Users
 * @property \App\Model\Table\RestaurantQuestionsTable $RestaurantQuestions
 * @property \App\Model\Table\OfficesTable $Offices
 * @method \App\Model\Entity\CheckersQuestions[]|\Cake\Datasource\ResultSetInterface paginate($object = null, array $settings = [])
 */
class RestaurantApplicationController extends AppController
{
    public $paginate = [
        'limit' => PAGINATE_DEFAULT,
        'order' => [
            'RestaurantQuestions.id' => 'asc'
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

    public function initialize(): void
    {
        parent::initialize();
        $this->loadModel('RestaurantQuestions');
        $this->loadModel('Offices');
        $this->pdfService = new PDFService();
        $this->sendMailService = new SendMailService();

        $this->loadComponent('Flash'); // Include the FlashComponent
    }

    /**
     * Restaurants' answers list
     *
     * @return \Cake\Http\Response|null|void Renders view
     */
    public function list()
    {
        $restaurantQuestions = $this->RestaurantQuestions->getListAnswer($this->RestaurantQuestions, ['fields' => ['id', 'modified_at', 'status']]);
        $this->set('restaurantQuestions',  $this->paginate($restaurantQuestions));
    }

    /**
     * Checker score restaurant application screen
     *
     * @return \Cake\Http\Response|null|void Renders view
     */
    public function score(int $id)
    {
        $restaurantQuestion = $this->RestaurantQuestions
            ->findById($id)
            ->contain('Offices.Businesses')
            ->first();

        if (!$restaurantQuestion) {
            throw new NotFoundException(__('Not found'));
        }

        if (is_string($restaurantQuestion->answer_questions)) {
            $answerQuestions = json_decode($restaurantQuestion->answer_questions, true);
        } else {
            $answerQuestions = $restaurantQuestion->answer_questions;
        }

        if($this->request->is(['put', 'post'])) {
            $dataRequest = array_diff_key($this->request->getData(), ['tab_item' => 'on']);

            //handle score data
            $this->handleScore($dataRequest);

            $restaurantQuestion = $this->RestaurantQuestions->patchEntity($restaurantQuestion, $dataRequest);

            if(empty($restaurantQuestion->getErrors())) {
                $data = array_merge(
                    array_map(function($item) {
                        return json_encode($item, JSON_FORCE_OBJECT);
                    }, $dataRequest),
                    ['status' => APPLICATION_STATUS_SCORED]
                );
                $data = array_filter($data, fn($value) => !is_null($value) && $value !== '');

                $restaurantQuestion = $this->RestaurantQuestions->patchEntity($restaurantQuestion, $data, ['validate' => false]);

                $this->RestaurantQuestions->save($restaurantQuestion);
                $this->redirect(['action' => 'total', 'id' => $id]);
            } else {
                $this->set('errors', $restaurantQuestion->getErrors());
            }
        }

        $this->set('restaurantQuestion',  $restaurantQuestion);
        $this->set('answerQuestions',  $answerQuestions);
    }

    /**
     * Checker show total point restaurant application screen
     *
     * @return \Cake\Http\Response|null|void Renders view
     */
    public function total(int $id)
    {
        $restaurantQuestion = $this->RestaurantQuestions
            ->findById($id)
            ->contain('Offices.Businesses')
            ->first();

        if (!$restaurantQuestion) {
            throw new NotFoundException(__('Not found'));
        }

        if (is_string($restaurantQuestion->answer_questions)) {
            $answerQuestions = json_decode($restaurantQuestion->answer_questions, true);
        } else {
            $answerQuestions = $restaurantQuestion->answer_questions;
        }

        if($this->request->is(['put', 'post'])) {
            $total = $this->request->getData('total');
            $session = $this->request->getSession();
            $session->write('total', $total);

            $this->redirect(['action' => 'sendMail', 'id' => $id]);
        }

        $this->set('restaurantQuestion', $restaurantQuestion);
        $this->set('answerQuestions',  $answerQuestions);
    }

    /**
     * Checker send mail to restaurant application
     *
     * @return \Cake\Http\Response|null|void Renders view
     */
    public function sendMail(int $id)
    {
        $restaurantQuestion = $this->RestaurantQuestions
            ->findById($id)
            ->select(['id', 'office_id', 'modified_at', 'total_score', 'status'])
            ->contain([
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
            ])
            ->first();

        $office = $this->Offices->findById($restaurantQuestion->office_id)->first();

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
                        'office_type'           => $restaurantQuestion->office->office_type,
                        'office_name'           => $restaurantQuestion->office->name,
                        'business_name'         => $restaurantQuestion->office->business->name,
                        'representative_name'   => $restaurantQuestion->office->business->representative_name,
                        'total_score'           => $restaurantQuestion->total_score,
                        'business_address'      => h($restaurantQuestion->office->business->prefecture)
                                                    . h($restaurantQuestion->office->business->city)
                                                    . h($restaurantQuestion->office->business->street),
                        'certification_date'    => date("Y年m月d日")
                    ]
                );

                if (in_array($rank, APPROVE_RANK_LIST)) {
                    $certificatePath            = $this->pdfService->convertHtmlToPdf($data);
                    $restaurantQuestion->status = APPLICATION_STATUS_APPROVED;
                } else {
                    $restaurantQuestion->status = APPLICATION_STATUS_REJECTED;
                }

                if ($restaurantQuestion->status === APPLICATION_STATUS_APPROVED && !$certificatePath) {
                    $this->Flash->error(ERROR_OCCUR);
                    $this->redirect(['action' => 'list']);
                } else {
                    $office->offices_certified_file_path = $certificatePath;
                    $restaurantQuestion->office_id = $office->id;

                    //get total score
                    $total = $this->request->getSession()->read('total');
                    $requestData = array_merge($this->request->getData(), ['total_score' => $total]);

                    $restaurantQuestion = $this->RestaurantQuestions->patchEntity($restaurantQuestion, $requestData);
                    if (!$restaurantQuestion->getErrors()) {
                        $conn = ConnectionManager::get('default');
                        $conn->begin();

                        try {
                            $conn->transactional(function ($conn) use ($restaurantQuestion, $office, $data) {
                                if ($this->Offices->save($office)) {
                                    $this->RestaurantQuestions->save($restaurantQuestion);

                                    $args = $restaurantQuestion;
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

        $this->set('restaurantQuestion', $restaurantQuestion);
    }

    /**
     * handle score answer
     *
     * @param array $dataRequest
     *
     * @return void
     */
    public function handleScore(array $dataRequest): void
    {
        $dataRequest['question_1_score'] = $this->getScoreMenuAnswer(MAX_MENU_QUESTION_1, 1, $dataRequest);
        $dataRequest['question_7_score'] = $this->getScoreMenuAnswer(MAX_MENU_QUESTION_7, 7, $dataRequest);

        $maxScoreQuestion1 = $this->handleMaxScoreOption(1, 46, $dataRequest);
        $maxScoreQuestion2And3 = $this->handleMaxScoreOption([2, 3], 20, $dataRequest);
        $maxScoreQuestion4To7 = $this->handleMaxScoreOption([4, 5, 6, 7], 24, $dataRequest);
        $maxScoreQuestion8 = $this->handleMaxScoreOption(8, 10, $dataRequest);

        $total = $maxScoreQuestion1 + $maxScoreQuestion2And3 + $maxScoreQuestion4To7 + $maxScoreQuestion8;

        $session = $this->request->getSession();
        $session->write('total', $total);
    }

    /**
     * Handle menu answers
     *
     * @param int $length
     * @param int $questionNo
     * @param array $dataRequest
     *
     * @return int score of answer type menu
     */
    public function getScoreMenuAnswer(int $length, int $questionNo, array $dataRequest): int
    {
        $answerQuestion = [];

        for ($i = 1; $i <= $length; $i++) {
            if (isset($dataRequest[sprintf("question_{$questionNo}_menu_%d_score", $i)])) {
                $answerQuestion[] = $dataRequest[sprintf("question_{$questionNo}_menu_%d_score", $i)];
            }
        }

        if (!$answerQuestion) return 0;
        $menusQuestion = cols_from_array($answerQuestion, ['score', 'local_ingredients_score']);
        $menusQuestionScore = array_sum(array_map('array_sum', $menusQuestion));

        $ingredientsQuestion = array_column($answerQuestion, 'ingredients');
        $ingredientsQuestionScore = array_sum(array_flatten($ingredientsQuestion));

        if ($questionNo == 1) {
            $menusQuestionScore = $menusQuestionScore <= 25 ? $menusQuestionScore : 25;
            $ingredientsQuestionScore = $ingredientsQuestionScore <= 21 ? $ingredientsQuestionScore : 21;
        }

        return $menusQuestionScore + $ingredientsQuestionScore;
    }

    /**
     * handle max score for answers
     *
     * @param mixed $quetionNo
     * @param int $max
     * @param array @dataRequest
     *
     * @return int $score
     */
    public function handleMaxScoreOption(mixed $questionNo, int $max, array $dataRequest): int
    {
        $dataHandle = [];

        $questionNo = is_array($questionNo) ? $questionNo : [$questionNo];

        foreach ($questionNo as $question) {
            if (!empty($dataRequest[sprintf("question_%d_score", $question)])) {
                $dataHandle[sprintf("question_%d_score", $question)] = $dataRequest[sprintf("question_%d_score", $question)];
            }
        }
        $dataHandle = array_map(fn($ans) => !is_array($ans) ? [$ans] : $ans, $dataHandle);

        $score = array_sum(array_map('array_sum', $dataHandle)) <= $max ? array_sum(array_map('array_sum', $dataHandle)) : $max;

        $session = $this->request->getSession();
        $session->write('max_of_question_' . implode("_", $questionNo), $score);

        return $score;
    }
}
