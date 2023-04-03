<?php
declare(strict_types=1);

namespace App\Controller\Farms;

use App\Controller\AppController;
use App\Errors\ApiErrorRenderer;
use App\Service\SendMailService;
use App\Service\UploadService;
use Cake\Core\Configure;
use Cake\Database\Query;
use Cake\Datasource\ConnectionManager;
use Cake\Http\Exception\NotFoundException;

/**
 * Application Controller
 * @property \App\Model\Table\UsersTable $Users
 * @property \App\Model\Table\FarmQuestionsTable $FarmQuestions
 * @property \App\Model\Table\OfficesTable $Offices
 * @method \App\Model\Entity\FarmsQuestions[]|\Cake\Datasource\ResultSetInterface paginate($object = null, array $settings = [])
 */
class ApplicationController extends AppController
{
    public function beforeFilter(\Cake\Event\EventInterface $event)
    {
        parent::beforeFilter($event);

        $this->Authentication->addUnauthenticatedActions([]);
        $this->viewBuilder()->setLayout("user/farm_layout");
    }

    public function initialize(): void
    {
        parent::initialize();
        $this->loadModel('FarmQuestions');
        $this->loadModel('Offices');
        $this->loadModel('Users');
        $this->loadComponent('Flash'); // Include the FlashComponent
    }

    /**
     * Index method
     *
     * @return \Cake\Http\Response|null|void Renders view
     */
    public function index()
    {
        $this->getUserLoggedIn(
            ['Offices.Businesses' => function (Query $query) {
                return $query->select(['representative_name']);
            }]
        );

        Configure::load('farm_questions', 'default');
        $farmQuestions = Configure::read('questions');

        $questionNo = (int) $this->request->getParam('screen');

        if (!array_key_exists($questionNo, $farmQuestions) ||
            !$farmQuestions[$questionNo]
        ) {
            return $this->redirect([
                'action' => 'index',
                'screen' => FARM_QUESTIONS_START
            ]);
        }

        $farmQuestion = json_decode(json_encode($farmQuestions[$questionNo]), FALSE);

        $this->set('questionNo', $questionNo);
        $this->set('farmQuestion', $farmQuestion);
        $this->set('total', count($farmQuestions));

        return $this->render('/Farms/Application/question');
    }

    /**
     *
     * @return \Cake\Http\Response|null|void Renders view
     */
    public function confirmation()
    {
        $this->getUserLoggedIn(
            ['Offices.Businesses' => function (Query $query) {
                return $query->select(['representative_name']);
            }]
        );

        Configure::load('farm_questions', 'default');
        $farmQuestions = Configure::read('questions');
        $total = count($farmQuestions);
        $farmQuestions = json_decode(json_encode($farmQuestions), FALSE);

        $this->set('farmQuestions', $farmQuestions);
        $this->set('total', $total);
    }

    /**
     *
     * @return \Cake\Http\Response|null|void Renders view
     */
    public function complete()
    {
        $this->getUserLoggedIn(
            ['Offices.Businesses' => function (Query $query) {
                return $query->select(['representative_name']);
            }]
        );
    }

    /**
     * Store method
     *
     * @return \Cake\Http\Response
     */
    public function store(): \Cake\Http\Response
    {
        $farmQuestions = $this->FarmQuestions->newEmptyEntity();

        $auth = $this->Authentication->getIdentity()->getOriginalData();
        $user = $this->Users->findById($auth->id)->contain('Offices')->first();
        $office = $user->office;

        $existQuestions = $this->FarmQuestions->getByOfficeId($this->FarmQuestions, ['office_id' => $office->id]);

        if ($existQuestions) {
            $farmQuestions = $existQuestions;
        }

        if ($this->request->is('post')) {
            $farmQuestions->office_id = $office->id;
            $farmQuestions = $this->FarmQuestions->patchEntity($farmQuestions, $this->request->getData());
            $farmQuestions->answer_questions = $this->request->getData('answer_questions');
            $farmQuestions->status = APPLICATION_STATUS_ANSWER;

            if (!$farmQuestions->getErrors() && !empty($farmQuestions->answer_questions)) {
                $conn = ConnectionManager::get('default');
                $conn->begin();

                try {
                    $conn->transactional(function ($conn) use ($farmQuestions, $office) {
                        if ($this->FarmQuestions->save($farmQuestions)) {
                            $office->offices_certified_rank = PENDING_VERIFICATION;
                            $this->Offices->save($office);

                            $conn->commit();
                        }
                    });
                } catch (\Cake\ORM\Exception\PersistenceFailedException $e) {
                    $conn->rollback();
                }
            }
        }
        $response = $this->response->withType('application/json')
            ->withStatus(200)
            ->withStringBody(json_encode([
                'success' => true,
                'message' => APPLICATION_INFORMATION_APPLIED
            ]));

        return $response;
    }

    /**
     * Display applied answers
     *
     * @return \Cake\Http\Response
     */
    public function show(): \Cake\Http\Response
    {
        $auth = $this->Authentication->getIdentity()->getOriginalData();

        $user = $this->Users->findById($auth->id)->contain('Offices')->first();
        $dataRet = null;

        if ($user->office) {
            $answers = $this->FarmQuestions->getByOfficeId($this->FarmQuestions, ['office_id' => $user->office->id]);
            $dataRet = isset($answers) && $answers->answer_questions ? json_decode($answers->answer_questions) : null;
        }

        $response = $this->response->withType('application/json')
            ->withStatus(200)
            ->withStringBody(json_encode([
                'data'    => $dataRet,
                'success' => true
            ]));

        return $response;
    }

    /**
     * Upload method
     *
     * @return \Cake\Http\Response
     */
    public function upload(UploadService $uploadService): \Cake\Http\Response
    {
        if ($this->request->is('post')) {
            $file = $this->request->getData('image');

            if ($file->getSize() > 0 && $file->getError() == 0) {
                $result = $uploadService->uploadAndConvertToPNG($file, OFFICE_PRODUCER);
            } else {
                return ApiErrorRenderer::prepareResponse(400, ERROR_OCCUR);
            }
        }

        $response = $this->response->withType('application/json')
            ->withStatus(200)
            ->withStringBody(json_encode([
                'data' => $result,
                'success' => true,
                'message' => IMAGE_UPLOAD_SUCCESSFULLY
            ]));

        return $response;
    }
}
