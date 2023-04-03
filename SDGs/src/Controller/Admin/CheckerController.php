<?php
declare(strict_types=1);

namespace App\Controller\Admin;

use App\Controller\AppController;
use Cake\Log\Log;

/**
 * Checker Controller
 *
 * @method \App\Model\Entity\Checker[]|\Cake\Datasource\ResultSetInterface paginate($object = null, array $settings = [])
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
     * Register a new account checker method
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
                if ($this->Users->save($checker)) {
                    $this->Flash->success(REGISTER_USER_SUCCESS);

                    return $this->redirect(['_name' => 'admin.checker.list']);
                }
            }
        }

        $this->set('checker', $checker);
    }

    /**
     * List of checker method
     *
     * @return \Cake\Http\Response|null|void Renders view
     */
    public function list()
    {
        $this->getUserLoggedIn();
        $checkers = $this->Users->find('all', [
            'conditions' => ['Users.role' => ROLE_CHECKER]
        ]);

        $this->set('checkers', $this->paginate($checkers));
    }

    /**
     * Edit method
     *
     * @param int $id
     * @return \Cake\Http\Response|null|void Renders view
     */
    public function edit($id)
    {
        $checker = $this->Users->findById($id)->where(['role' => ROLE_CHECKER])->firstOrFail();

        if ($this->request->is(['post', 'put', 'patch'])) {
            $checker = $this->Users->patchEntity($checker, $this->request->getData(), ['validate' => 'checker']);

            if (!$checker->hasErrors()) {
                if (!empty($this->request->getData('new_password'))) {
                    $checker->password = $this->request->getData('new_password');
                }

                if ($this->Users->save($checker)) {
                    $this->Flash->success(UPDATE_CHECKER_SUCCESS);

                    return $this->redirect(['_name' => 'admin.checker.list']);
                }
            }
        }

        $this->set('checker', $checker);
    }

    /**
     * Detail of checker
     *
     * @param int $id
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
     * Delete admin
     *
     * @param int $id
     *
     * @return \Cake\Http\Response|null|void Renders view
     */
    public function delete(int $id)
    {
        $checker = $this->Users->findById($id)->where(['role' => ROLE_CHECKER])->firstOrFail();
        $user = $this->Authentication->getIdentity();

        if ($user->id == $id) {
            return $this->redirect(['_name' => 'admin.checker.list']);
        }

        if ($this->request->is('delete')) {
            try {
                $this->Users->deleteOrFail($checker);
                $this->Flash->error(DELETE_CHECKER_SUCCESS);

                return $this->redirect(['_name' => 'admin.checker.list']);
            } catch (\Cake\ORM\Exception\PersistenceFailedException $e) {
                Log::write('error', __('ERROR WHEN DELETE ADMIN ACCOUNT WITH ID = {0}', $e->getEntity()->id));
            }
        }

        $this->set('checker', $checker);
    }
}
