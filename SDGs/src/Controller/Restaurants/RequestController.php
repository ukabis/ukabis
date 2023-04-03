<?php
declare(strict_types=1);

namespace App\Controller\Restaurants;

use App\Controller\AppController;

/**
 * RequestController Controller
 *
 * @property \App\Model\Table\UsersTable $Users
 * @property \App\Model\Table\RestaurantRequestsTable $RestaurantRequests
 * @method \App\Model\Entity\Home[]|\Cake\Datasource\ResultSetInterface paginate($object = null, array $settings = [])
 */
class RequestController extends AppController
{
    public function beforeFilter(\Cake\Event\EventInterface $event)
    {
        parent::beforeFilter($event);
        // 認証を必要としないログインアクションを構成し、
        // 無限リダイレクトループの問題を防ぎます
        // アカウント作成後は ,'add' を削除
        $this->Authentication->addUnauthenticatedActions([]);
        $this->viewBuilder()->setLayout("user/restaurant_layout");
    }

    public function initialize(): void
    {
        parent::initialize();
        $this->loadModel('Users');
        $this->loadModel('RestaurantRequests');
        $this->loadComponent('Flash'); // Include the FlashComponent
    }

    /**
     * Register method
     *
     * @return \Cake\Http\Response|null|void Renders view
     */
    public function register()
    {
        $user = $this->Users->findById($this->Authentication->getIdentity()->id)->contain('Offices')->firstOrFail();

        $request = $this->RestaurantRequests->newEmptyEntity();
        $listRequest = $this->RestaurantRequests->getListRequestByOfficeId($this->RestaurantRequests, $user->office->id);

        if ($this->request->is('post')) {
            $data = $this->request->getData();
            $data['office_id'] = $user->office->id;
            if (!empty($data['food']) || !empty($data['comment'])) {
                $request = $this->RestaurantRequests->patchEntity($request, $data);
                if (!$request->getErrors()) {
                    if ($this->RestaurantRequests->save($request)) {
                        $this->Flash->success(REGISTER_REQUEST_SUCCESS);
                        return $this->redirect(['action' => 'list']);
                    }
                }
            } else {
                return $this->redirect(['action' => 'list']);
            }
        }
        $this->set('listRequest', $listRequest);
        $this->set('request', $request);
    }

    /**
     * List request
     *
     * @return \Cake\Http\Response|null|void Renders view
     */
    public function list()
    {
        $user = $this->Users->findById($this->Authentication->getIdentity()->id)->contain('Offices')->firstOrFail();
        $listRequest = $this->RestaurantRequests->getListRequestByOfficeId($this->RestaurantRequests, $user->office->id);

        $this->set('listRequest', $listRequest);
    }
}
