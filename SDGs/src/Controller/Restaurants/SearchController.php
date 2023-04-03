<?php
declare(strict_types=1);

namespace App\Controller\Restaurants;

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
        $this->viewBuilder()->setLayout("user/restaurant_layout");
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
        $offices = $this->Offices->searchFarmByCondition($this->Offices, $this->request->getQuery());

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
        $farmRequest = null;
        if ($this->request->getQuery('requestId')) {
            $farmRequest = $this->FarmRequests->findById($this->request->getQuery('requestId'))->contain('Offices')->first();
        }
        if ($farmRequest && $farmRequest->office->id != $office->id) {
            $farmRequest = null;
        }

        $this->setQueryParam();
        $this->set('office', $office);
        $this->set('farmRequest', $farmRequest);
    }

    /**
     * Detail method
     *
     * @return \Cake\Http\Response|null|void Renders view
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
