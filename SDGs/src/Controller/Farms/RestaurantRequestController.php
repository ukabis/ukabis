<?php
declare(strict_types=1);

namespace App\Controller\Farms;

use App\Controller\AppController;
use App\Errors\ApiErrorRenderer;
use App\Service\SendMailService;
use Cake\Http\Response;
use Cake\Log\Log;

/**
 * RestaurantRequestController Controller
 *
 *  @property \App\Model\Table\UsersTable $Users
 * @property \App\Model\Table\OfficesTable $Offices
 * @property \App\Model\Table\RestaurantRequestsTable $RestaurantRequests
 * @method \App\Model\Entity\Home[]|\Cake\Datasource\ResultSetInterface paginate($object = null, array $settings = [])
 */
class RestaurantRequestController extends AppController
{
    public $paginate = [
        'limit' => PAGINATE_DEFAULT,
        'order' => [
            'RestaurantRequests.id' => 'asc'
        ]
    ];

    public function beforeFilter(\Cake\Event\EventInterface $event)
    {
        parent::beforeFilter($event);
        // 認証を必要としないログインアクションを構成し、
        // 無限リダイレクトループの問題を防ぎます
        // アカウント作成後は ,'add' を削除
        $this->Authentication->addUnauthenticatedActions([]);
        $this->viewBuilder()->setLayout("user/farm_layout");
    }

    public function initialize(): void
    {
        parent::initialize();
        $this->loadModel('Offices');
        $this->loadModel('Users');
        $this->loadModel('FarmRequests');
        $this->loadModel('RestaurantRequests');
        $this->loadComponent('Flash'); // Include the FlashComponent
    }

    /**
     * Search method
     *
     * @return \Cake\Http\Response|null|void Renders view
     */
    public function search()
    {
        $user = $this->Users->findById($this->Authentication->getIdentity()->id)->contain('Offices')->firstOrFail();
        $dataFarmRequests = $this->FarmRequests->getListRequestByOfficeId($this->FarmRequests, $user->office->id, ['FarmRequests.food'])->toArray();
        $resultRestaurantRequests = [];

        if (count($dataFarmRequests)) {
            $resultRestaurantRequests = $this->paginate($this->RestaurantRequests->searchByFarmRequest($this->RestaurantRequests, $dataFarmRequests));
        }

        $this->set('dataFarmRequests', $dataFarmRequests);
        $this->set('resultRestaurantRequests', $resultRestaurantRequests);
    }

    /**
     * Detail method
     *
     * @param $id request Id
     * @return \Cake\Http\Response|null|void Renders view
     */
    public function detail(int $id)
    {
        $restaurantRequest = $this->RestaurantRequests->findById($id)->contain('Offices')->first();
        if (!$restaurantRequest) {
            return $this->redirect(['controller' => 'Request', 'action' => 'search']);
        }
        $office = $this->Offices->findById($restaurantRequest->office_id)->contain('Businesses')->first();

        $this->set('office', $office);
        $this->set('restaurantRequest', $restaurantRequest);
    }

    /**
     * Inquiry method
     *
     * @return Response
     */
    public function inquiry(SendMailService $sendMailService): Response
    {
        $this->request->allowMethod(['post']);

        try {
            $officeId = (int) $this->request->getData('destination_office_id');
            $sendMailService->inquiry($this, $officeId);

            $response = $this->response->withType('application/json')
                ->withStatus(200)
                ->withStringBody(json_encode([
                    'success' => true,
                    'message' => SEND_MAIL_SUCCESSFULLY
                ]));
        } catch (\Throwable $th) {
            Log::write('error',
                sprintf("----- ERROR WHEN SENDING EMAIL ----- \n %s",
                    $th->getMessage()
                )
            );

            $response = ApiErrorRenderer::prepareResponse(400, ERROR_OCCUR);
        }

        return $response;
    }
}
