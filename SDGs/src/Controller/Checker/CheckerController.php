<?php

declare(strict_types=1);

namespace App\Controller\Checker;

use App\Controller\AppController;
use Cake\Database\Query;

/**
 * CheckerController Controller
 *
 * @property \App\Model\Table\UsersTable $Users
 * @method \App\Model\Entity\Home[]|\Cake\Datasource\ResultSetInterface paginate($object = null, array $settings = [])
 */
class CheckerController extends AppController
{
    public $paginate = [
        'limit' => PAGINATE_DEFAULT,
        'order' => [
            'Users.id' => 'asc'
        ]
    ];

    public function beforeFilter(\Cake\Event\EventInterface $event)
    {
        parent::beforeFilter($event);
        // 認証を必要としないログインアクションを構成し、
        // 無限リダイレクトループの問題を防ぎます
        // アカウント作成後は ,'add' を削除

        $this->Authentication->addUnauthenticatedActions([]);
        $this->viewBuilder()->setLayout("user/checker_layout");
    }

    public function initialize(): void
    {
        parent::initialize();
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
        //
    }

    /**
     * Register method
     *
     * @return \Cake\Http\Response|null|void Renders view
     */
    public function register()
    {
        $checker = $this->Users->newEmptyEntity();
        if ($this->request->is('post')) {
            $checker->role = ROLE_CHECKER;
            $checker = $this->Users->patchEntity($checker, $this->request->getData(), ['validate' => 'checker']);
            if (!$checker->getErrors()) {
                $checker->verification_code = (string) mt_rand(100000, 999999);
                if ($this->Users->save($checker)) {
                    $this->Flash->success(REGISTER_USER_SUCCESS);
                    return $this->redirect(['controller' => 'Checker', 'action' => 'list']);
                }
            }
        }
        $this->set('checker', $checker);
    }

    /**
     * List checker method
     *
     * @return \Cake\Http\Response|null|void Renders view
     */
    public function list()
    {
        $this->getUserLoggedIn();
        $listChecker = $this->Users->find('all')
            ->where(['Users.role =' => ROLE_CHECKER]);

        $this->set('listChecker', $this->paginate($listChecker));
    }

    /**
     * Detail of checker
     *
     * @return \Cake\Http\Response|null|void Renders view
     */
    public function detail(int $id)
    {
        $this->getUserLoggedIn();
        $checker = $this->Users->findById($id)->where(['role' => ROLE_CHECKER])->firstOrFail();

        $this->set('checker', $checker);
    }

    /**
     * Edit checker
     *
     * @return \Cake\Http\Response|null|void Renders view
     */
    public function edit(int $id)
    {
        $checker = $this->Users->findById($id)->where(['role' => ROLE_CHECKER])->firstOrFail();

        if ($this->request->is(['post', 'put'])) {
            $checker = $this->Users->patchEntity($checker, $this->request->getData(), ['validate' => 'checker']);
            if (empty($checker->getErrors())) {
                if (!empty($this->request->getData('new_password'))) {
                    $checker->password = $this->request->getData('new_password');
                }

                if ($this->Users->save($checker)) {
                    $this->Flash->success(UPDATE_CHECKER_SUCCESS);
                    return $this->redirect(['controller' => 'Checker', 'action' => 'list']);
                }
            }
        }

        $this->set('checker', $checker);
    }

    /**
     * Delete checker
     *
     * @return \Cake\Http\Response|null|void Renders view
     */
    public function delete(int $id)
    {
        $checker = $this->Users->findById($id)->where(['role' => ROLE_CHECKER])->firstOrFail();
        $user = $this->Authentication->getIdentity();

        if ($user->id == $id) {
            return $this->redirect(['controller' => 'Checker', 'action' => 'list']);
        }

        if ($this->request->is('delete')) {
            try {
                $this->Users->deleteOrFail($checker);

                $this->Flash->error(DELETE_CHECKER_SUCCESS);
                return $this->redirect(['controller' => 'Checker', 'action' => 'list']);
            } catch (\Cake\ORM\Exception\PersistenceFailedException $e) {
                echo 'ERROR WHEN DELETE CHECKER WITH ID = ' . $e->getEntity()->id;
            }
        }

        $this->set('checker', $checker);
    }
}
