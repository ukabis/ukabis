<?php

namespace App\Controller\Farms;

use App\Controller\AppController;
use App\Service\DownloadService;

/**
 * Download Controller
 *
 * @property \App\Model\Table\UsersTable $Users
 * @method \App\Model\Entity\Home[]|\Cake\Datasource\ResultSetInterface paginate($object = null, array $settings = [])
 */
class DownloadController extends AppController
{
    public function beforeFilter(\Cake\Event\EventInterface $event)
    {
        parent::beforeFilter($event);

        $this->Authentication->addUnauthenticatedActions([]);
    }

    public function initialize(): void
    {
        parent::initialize();
        $this->loadModel('Users');
        $this->loadComponent('Flash'); // Include the FlashComponent
        $this->downloadService = new DownloadService();
    }

    /**
     * Download method
     *
     * @return \Cake\Http\Response|null
     */
    public function downloadCertificate()
    {
        $this->autoRender = false;
        $this->request->allowMethod(['post']);
        $user = $this->getUserLoggedIn(['Offices']);
        $targetFile = (int) $this->request->getData('target_file');
        $filePath = '';

        if ($user->office &&
            in_array($user->office->offices_certified_rank, APPROVE_RANK_LIST) &&
            $user->office->offices_certified_file_path
        ) {
            switch ($targetFile) {
                case FILE_MARK:
                    $filePath = WWW_ROOT . $user->office->certificate['mark_image'];
                    break;

                case FILE_CERTIFICATE:
                    $filePath = WWW_ROOT . $user->office->offices_certified_file_path;
                    break;

                default:
                    break;
            }
        }

        $result = $this->downloadService->download($filePath);

        if (!$result) {
            $this->Flash->error(ERROR_OCCUR);
            return $this->redirect(['controller' => 'Farm', 'action' => 'download']);
        }
    }
}
