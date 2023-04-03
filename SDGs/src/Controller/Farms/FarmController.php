<?php
declare(strict_types=1);

namespace App\Controller\Farms;

use App\Controller\AppController;
use Cake\Database\Query;

/**
 * FarmController Controller
 *
 * @property \App\Model\Table\UsersTable $Users
 * @property \App\Model\Table\FarmRequestsTable $FarmRequests
 * @method \App\Model\Entity\Home[]|\Cake\Datasource\ResultSetInterface paginate($object = null, array $settings = [])
 */
class FarmController extends AppController
{
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
        $this->loadModel('Users');
        $this->loadModel('FarmRequests');
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
    }

    /**
     * Edit method
     *
     * @return \Cake\Http\Response|null|void Renders view
     */
    public function edit()
    {
        $this->getUserLoggedIn(
            ['Offices.Businesses' => function (Query $query) {
                return $query->select(['representative_name']);
            }]
        );
    }

    /**
     * Detail method
     *
     * @return \Cake\Http\Response|null|void Renders view
     */
    public function detail()
    {
        $this->getUserLoggedIn(['Offices.Businesses']);
        $this->getLatestRequestsOffice($this->FarmRequests);
    }

    /**
     *
     * @return \Cake\Http\Response|null|void Renders view
     */
    public function download()
    {
        $user = $this->getUserLoggedIn(
            ['Offices.Businesses' => function (Query $query) {
                return $query->select(['representative_name']);
            }]
        );

        if (!in_array($user->office->offices_certified_rank, APPROVE_RANK_LIST)) {
            return  $this->redirect(['action' => 'edit']);
        }
    }
}
