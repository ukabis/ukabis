<?php
declare(strict_types=1);

namespace App\Controller\Restaurants;

use App\Controller\AppController;
use App\Errors\ApiErrorRenderer;
use App\Service\UploadService;
use Cake\Core\Configure;
use Cake\Database\Query;
use Cake\Datasource\ConnectionManager;
use Cake\Http\Exception\NotFoundException;

/**
 * Application Controller
 * @property \App\Model\Table\UsersTable $Users
 * @property \App\Model\Table\RestaurantQuestionsTable $RestaurantQuestions
 * @property \App\Model\Table\OfficesTable $Offices
 * @method \App\Model\Entity\RestaurantsQuestions[]|\Cake\Datasource\ResultSetInterface paginate($object = null, array $settings = [])
 */
class ApplicationController extends AppController
{
    public function beforeFilter(\Cake\Event\EventInterface $event)
    {
        parent::beforeFilter($event);

        $this->Authentication->addUnauthenticatedActions([]);
        $this->viewBuilder()->setLayout("user/restaurant_layout");
    }

    public function initialize(): void
    {
        parent::initialize();
        $this->loadModel('RestaurantQuestions');
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

        $restaurantQuestions = Configure::read('restaurantQuestions');
        $questionNo = (int) $this->request->getParam('screen');

        if (!array_key_exists($questionNo, $restaurantQuestions) ||
            !$restaurantQuestions[$questionNo]
        ) {
            return $this->redirect([
                'action' => 'index',
                'screen' => RESTAURANT_QUESTIONS_START
            ]);
        }

        $this->set('menuComp', json_encode($restaurantQuestions[$questionNo]));

        $restaurantQuestion = json_decode(json_encode($restaurantQuestions[$questionNo]), FALSE);

        $this->set('questionNo', $questionNo);
        $this->set('restaurantQuestion', $restaurantQuestion);
        $this->set('total', count($restaurantQuestions));

        return $this->render("/Restaurants/Application/question");
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

        $restaurantQuestions = Configure::read('restaurantQuestions');
        $total = count($restaurantQuestions);
        $restaurantQuestions = json_decode(json_encode($restaurantQuestions), FALSE);

        $this->set('restaurantQuestions', $restaurantQuestions);
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
        $restaurantQuestions = $this->RestaurantQuestions->newEmptyEntity();

        $auth = $this->Authentication->getIdentity()->getOriginalData();
        $user = $this->Users->findById($auth->id)->contain('Offices')->first();
        $office = $user->office;

        $existQuestions = $this->RestaurantQuestions->getByOfficeId($this->RestaurantQuestions, ['office_id' => $office->id]);

        if ($existQuestions) {
            $restaurantQuestions = $existQuestions;
        }

        if ($this->request->is('post')) {
            $restaurantQuestions->office_id = $office->id;
            $restaurantQuestions = $this->RestaurantQuestions->patchEntity($restaurantQuestions, $this->request->getData());
            $restaurantQuestions->answer_questions = $this->request->getData('answer_questions');
            $restaurantQuestions->status = APPLICATION_STATUS_ANSWER;

            if (!$restaurantQuestions->getErrors() && !empty($restaurantQuestions->answer_questions)) {
                $conn = ConnectionManager::get('default');
                $conn->begin();

                try {
                    $conn->transactional(function ($conn) use ($restaurantQuestions, $office) {
                        if ($this->RestaurantQuestions->save($restaurantQuestions)) {
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
            $answers = $this->RestaurantQuestions->getByOfficeId($this->RestaurantQuestions, ['office_id' => $user->office->id]);
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
                $result = $uploadService->uploadAndConvertToPNG($file, OFFICE_RESTAURANT);
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

