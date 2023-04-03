<?php

declare(strict_types=1);

namespace App\Controller\Admin;

use App\Controller\AppController;
use Cake\Database\Query;
use Cake\Log\Log;

/**
 * AdminController Controller
 *
 * @property \App\Model\Table\UsersTable $Users
 * @method \App\Model\Entity\Home[]|\Cake\Datasource\ResultSetInterface paginate($object = null, array $settings = [])
 */
class AdminController extends AppController
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
        $this->Authentication->addUnauthenticatedActions([]);
        $this->viewBuilder()->setLayout("user/admin_layout");
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
     * Register a new account admin method
     *
     * @return \Cake\Http\Response|null|void Renders view
     */
    public function register()
    {
        $admin = $this->Users->newEmptyEntity();

        if ($this->request->is('post')) {
            $admin->role = ROLE_ADMIN;
            $admin = $this->Users->patchEntity($admin, $this->request->getData(), ['validate' => 'checker']);

            if (!$admin->getErrors()) {
                if ($this->Users->save($admin)) {
                    $this->Flash->success(REGISTER_USER_SUCCESS);

                    return $this->redirect(['_name' => 'admin.list']);
                }
            }
        }

        $this->set('admin', $admin);
    }

    /**
     * List of admin method
     *
     * @return \Cake\Http\Response|null|void Renders view
     */
    public function list()
    {
        $this->getUserLoggedIn();
        $admins = $this->Users->find('all', [
            'conditions' => ['Users.role' => ROLE_ADMIN]
        ]);

        $this->set('admins', $this->paginate($admins));
    }

    /**
     * Edit method
     *
     * @param int $id
     * @return \Cake\Http\Response|null|void Renders view
     */
    public function edit($id)
    {
        $admin = $this->Users->findById($id)->where(['role' => ROLE_ADMIN])->firstOrFail();

        if ($this->request->is(['post', 'put', 'patch'])) {
            $admin = $this->Users->patchEntity($admin, $this->request->getData(), ['validate' => 'checker']);

            if (!$admin->hasErrors()) {
                if (!empty($this->request->getData('new_password'))) {
                    $admin->password = $this->request->getData('new_password');
                }

                if ($this->Users->save($admin)) {
                    $this->Flash->success(UPDATE_ADMIN_SUCCESS);

                    return $this->redirect(['_name' => 'admin.list']);
                }
            }
        }

        $this->set('admin', $admin);
    }

    /**
     * Detail of admin
     *
     * @param int $id
     *
     * @return \Cake\Http\Response|null|void Renders view
     */
    public function detail(int $id)
    {
        $this->getUserLoggedIn();
        $admin = $this->Users->findById($id)->where(['role' => ROLE_ADMIN])->firstOrFail();

        $this->set('admin', $admin);
    }

    /**
     * Delete admin
     *
     * @param int $id
     *
     * @return \Cake\Http\Response|null|void Renders view
     */
    public function delete(int $id)
    {
        $admin = $this->Users->findById($id)->where(['role' => ROLE_ADMIN])->firstOrFail();
        $user = $this->Authentication->getIdentity();

        if ($user->id == $id) {
            return $this->redirect(['_name' => 'admin.list']);
        }

        if ($this->request->is('delete')) {
            try {
                $this->Users->deleteOrFail($admin);
                $this->Flash->error(DELETE_ADMIN_SUCCESS);

                return $this->redirect(['_name' => 'admin.list']);
            } catch (\Cake\ORM\Exception\PersistenceFailedException $e) {
                Log::write('error', __('ERROR WHEN DELETE ADMIN ACCOUNT WITH ID = {0}', $e->getEntity()->id));
            }
        }

        $this->set('admin', $admin);
    }
}
