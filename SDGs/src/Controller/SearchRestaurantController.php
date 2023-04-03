<?php
declare(strict_types=1);

namespace App\Controller;

use App\Controller\AppController;
use Cake\Database\Query;

/**
 * SearchRestaurantController Controller
 *
 * @property \App\Model\Table\OfficesTable $Offices
 * @method \App\Model\Entity\Home[]|\Cake\Datasource\ResultSetInterface paginate($object = null, array $settings = [])
 */
class SearchRestaurantController extends AppController
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
        $this->Authentication->addUnauthenticatedActions(['list', 'detail']);
        $this->viewBuilder()->setLayout("user/restaurant_layout");
    }

    public function initialize(): void
    {
        parent::initialize();
        $this->loadModel('Offices');
    }

    /**
     * List method
     *
     * @return \Cake\Http\Response|null|void
     */
    public function list()
    {
        $offices = $this->Offices->searchOfficeByCondition(
            $this->Offices,
            OFFICE_RESTAURANT,
            $this->request->getQuery('restaurant') ?? []
        );

        $this->setQueryParam();
        $this->set('offices', $this->paginate($offices));
    }

    /**
     * Detail method
     *
     * @return \Cake\Http\Response|null|void Renders view
     */
    public function detail(int $id)
    {
        $office = $this->Offices->findById($id)->contain('Businesses')->first();
        if (!$office) {
            return $this->redirect(['controller' => 'SearchRestaurant', 'action' => 'list']);
        }

        $this->setQueryParam();
        $this->set('office', $office);
    }

    /**
     * Set param
     *
     * @return \Cake\Http\Response|null|void Renders view
     */
    private function setQueryParam()
    {
        $query = $this->request->getUri()->getUri()->getQuery();

        $this->set('query', $query);
    }
}
