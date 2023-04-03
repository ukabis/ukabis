<?php
declare(strict_types=1);

namespace App\Controller;

use App\Controller\AppController;
use Cake\Database\Query;

/**
 * SearchFarmController Controller
 *
 * @property \App\Model\Table\OfficesTable $Offices
 * @property \App\Model\Table\FarmRequestsTable $FarmRequests
 * @method \App\Model\Entity\Home[]|\Cake\Datasource\ResultSetInterface paginate($object = null, array $settings = [])
 */
class SearchFarmController extends AppController
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
        $this->viewBuilder()->setLayout("user/farm_layout");
    }

    public function initialize(): void
    {
        parent::initialize();
        $this->loadModel('Offices');
        $this->loadModel('FarmRequests');
    }

    /**
     * List method
     *
     * @return \Cake\Http\Response|null|void Renders view
     */
    public function list()
    {
        $offices = $this->Offices->searchOfficeByCondition(
            $this->Offices,
            OFFICE_PRODUCER,
            $this->request->getQuery('farm') ?? []
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
            return $this->redirect(['controller' => 'SearchFarm', 'action' => 'list']);
        }
        $farmRequest = null;
        if ($office->id) {
            $farmRequest = $this->FarmRequests
                ->findByOfficeId($office->id)
                ->contain('Offices')
                ->order('FarmRequests.created_at DESC')
                ->first();
        }

        $this->setQueryParam();
        $this->set('office', $office);
        $this->set('farmRequest', $farmRequest);
    }

    /**
     * Set param
     *
     * @return \Cake\Http\Response|null|void
     */
    private function setQueryParam()
    {
        $query = $this->request->getUri()->getUri()->getQuery();

        $this->set('query', $query);
    }
}
