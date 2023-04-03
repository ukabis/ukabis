<?php
declare(strict_types=1);

namespace App\Controller\Farms;

use App\Controller\AppController;
use Cake\Database\Query;

/**
 * SearchController Controller
 *
 * @property \App\Model\Table\UsersTable $Users
 * @property \App\Model\Table\OfficesTable $Offices
 * @property \App\Model\Table\RestaurantRequestsTable $RestaurantRequests
 * @method \App\Model\Entity\Home[]|\Cake\Datasource\ResultSetInterface paginate($object = null, array $settings = [])
 */
class SearchController extends AppController
{
    public $paginate = [
        'limit' => PAGINATE_DEFAULT,
        'order' => [
            'Offices.id' => 'asc'
        ]
    ];
    public function beforeFilter(\Cake\Event\EventInterface $event)
    {
        parent::beforeFilter($event);
        $this->Authentication->addUnauthenticatedActions([]);
        $this->viewBuilder()->setLayout("user/farm_layout");
    }

    public function initialize(): void
    {
        parent::initialize();
        $this->loadModel('Users');
        $this->loadModel('Offices');
        $this->loadModel('FarmRequests');
        $this->loadModel('RestaurantRequests');
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
     * List method
     *
     * @return \Cake\Http\Response|null|void Renders view
     */
    public function list()
    {
        $offices = $this->Offices->searchRestaurantByCondition($this->Offices, $this->request->getQuery());

        $this->setQueryParam();
        $this->set('offices', $this->paginate($offices));
    }

    /**
     * Detail method
     *
     * @return \Cake\Http\Response|null|void Renders view
     */
    public function detail(int $officeId)
    {
        $office = $this->Offices->findById($officeId)->contain('Businesses')->first();
        if (!$office) {
            return $this->redirect(['controller' => 'Search', 'action' => 'list']);
        }
        $restaurantRequest = null;
        if ($this->request->getQuery('requestId')) {
            $restaurantRequest = $this->RestaurantRequests->findById($this->request->getQuery('requestId'))->contain('Offices')->first();
        }
        if ($restaurantRequest && $restaurantRequest->office->id != $office->id) {
            $restaurantRequest = null;
        }

        $this->setQueryParam();
        $this->set('office', $office);
        $this->set('restaurantRequest', $restaurantRequest);
    }

    /**
     * Set param request
     *
     * @return \Cake\Http\Response|null|void
     */
    private function setQueryParam()
    {
        $page = $this->request->getQuery('page') ?? $this->request->getQuery('amp;page');
        $area = $this->request->getQuery('area') ?? $this->request->getQuery('amp;area');
        $keyword = $this->request->getQuery('keyword') ?? $this->request->getQuery('amp;keyword');

        $this->set('page', $page);
        $this->set('area', $area);
        $this->set('keyword', $keyword);
    }
}
