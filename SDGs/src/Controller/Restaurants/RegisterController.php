<?php
declare(strict_types=1);

namespace App\Controller\Restaurants;

use App\Controller\AppController;

/**
 * Home Controller
 *
 * @property \App\Model\Table\UsersTable $Users
 * @method \App\Model\Entity\Home[]|\Cake\Datasource\ResultSetInterface paginate($object = null, array $settings = [])
 */
class RegisterController extends AppController
{
    public function beforeFilter(\Cake\Event\EventInterface $event)
    {
        parent::beforeFilter($event);
        // 認証を必要としないログインアクションを構成し、
        // 無限リダイレクトループの問題を防ぎます
        // アカウント作成後は ,'add' を削除
        $this->Authentication->addUnauthenticatedActions(['register']);
        $this->viewBuilder()->setLayout("user/restaurant_layout");
    }

    public function initialize(): void
    {
        parent::initialize();
        $this->loadModel('Users');
        $this->loadComponent('Flash'); // Include the FlashComponent
    }

    /**
     * Register method
     *
     * @return \Cake\Http\Response|null|void Renders view
     */
    public function register()
    {
        $restaurant = $this->Users->newEmptyEntity();
        if ($this->request->is('post')) {
            $restaurant->role = ROLE_RESTAURANT;
            $restaurant = $this->Users->patchEntity($restaurant, $this->request->getData());
            if (!$restaurant->getErrors()) {
                $restaurant->verification_code = (string) mt_rand(100000, 999999);
                if ($this->Users->save($restaurant)) {
                    $this->Flash->success(REGISTER_USER_SUCCESS);
                    $authUser = $this->Users->get($restaurant->id);
                    // Log user in using Auth
                    $this->Authentication->setIdentity($authUser);
                    return $this->redirect(['controller' => 'President', 'action' => 'register']);
                }
            }
        }
        $restaurant->set('verification_code', null);
        $this->set('restaurant', $restaurant);
    }
}
