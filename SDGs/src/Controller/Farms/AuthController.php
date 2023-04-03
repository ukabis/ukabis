<?php
declare(strict_types=1);

namespace App\Controller\Farms;

use App\Controller\AppController;

/**
 * AuthController Controller
 *
 * @property \App\Model\Table\UsersTable $Users
 * @method \App\Model\Entity\Home[]|\Cake\Datasource\ResultSetInterface paginate($object = null, array $settings = [])
 */
class AuthController extends AppController
{
    public function beforeFilter(\Cake\Event\EventInterface $event)
    {
        parent::beforeFilter($event);
        // 認証を必要としないログインアクションを構成し、
        // 無限リダイレクトループの問題を防ぎます
        // アカウント作成後は ,'add' を削除
        $this->Authentication->addUnauthenticatedActions(['login']);
        $this->viewBuilder()->setLayout("user/common_layout");
    }

    public function initialize(): void
    {
        parent::initialize();

        $this->loadComponent('Flash'); // Include the FlashComponent
    }

    /**
     * Index method
     *
     * @return \Cake\Http\Response|null|void Renders view
     */
    public function login()
    {
        $this->request->allowMethod(['get', 'post']);
        $result = $this->Authentication->getResult();

        // POST, GET を問わず、ユーザーがログインしている場合はリダイレクトします
        if ($result && $result->isValid()) {
            // ログインロール判定
            $user = $this->Authentication->getIdentity();
            $role = $user->role;

            // ロールが管理者であれば管理者メニューへ
            if ($role === ROLE_FARMER) {
                // redirect to /articles after login success
                $redirect = $this->request->getQuery('redirect', [
                    'controller' => 'Farm',
                    'action' => 'index',
                ]);
            // ロールが農家であれば農家回答画面に
            } else {
                $this->Flash->error(LOGIN_FAILED_MESSAGE);
                return $this->logout();
            }

            return $this->redirect($redirect);
        }
        // ユーザーが submit 後、認証失敗した場合は、エラーを表示します
        if ($this->request->is('post') && !$result->isValid()) {
            $this->Flash->error(LOGIN_FAILED_MESSAGE);
        }
    }

    /**
     * Logout method
     *
     * @return \Cake\Http\Response|null|void Renders view
     */
    public function logout()
    {
        $result = $this->Authentication->getResult();
        if ($result->isValid()) {
            $this->Authentication->logout();
        }
        return $this->redirect(['controller' => 'Auth', 'action' => 'login']);
    }
}
