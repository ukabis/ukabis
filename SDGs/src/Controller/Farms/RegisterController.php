<?php
declare(strict_types=1);

namespace App\Controller\Farms;

use App\Controller\AppController;
use App\Model\Entity\User;

/**
 * RegisterController Controller
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
        $this->viewBuilder()->setLayout("user/farm_layout");
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
        $farm = $this->Users->newEmptyEntity();
        if ($this->request->is('post')) {
            $farm = $this->Users->patchEntity($farm, $this->request->getData());
            $farm->role = ROLE_FARMER;
            if (!$farm->getErrors()) {
                $farm->verification_code = (string) mt_rand(100000, 999999);
                if ($this->Users->save($farm)) {
                    $this->Flash->success(REGISTER_USER_SUCCESS);
                    $authUser = $this->Users->get($farm->id);
                    // Log user in using Auth
                    $this->Authentication->setIdentity($authUser);
                    return $this->redirect(['controller' => 'President', 'action' => 'register']);
                }
            }
        }

        $farm->set('verification_code', null);
        $this->set('farm', $farm);
    }
}
