<?php
declare(strict_types=1);

namespace App\Controller\Farms;

use App\Controller\AppController;
use App\Service\UploadService;
use Cake\Datasource\ConnectionManager;

/**
 * President Controller
 *
 * @property \App\Model\Table\UsersTable $Users
 * @property \App\Model\Table\BusinessesTable $Businesses
 * @property \App\Model\Table\OfficesTable $Offices
 * @method \App\Model\Entity\Home[]|\Cake\Datasource\ResultSetInterface paginate($object = null, array $settings = [])
 */
class PresidentController extends AppController
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
        $this->loadModel('Businesses');
        $this->loadModel('Offices');
        $this->uploadService = new UploadService();
        $this->loadComponent('Flash'); // Include the FlashComponent
    }

    /**
     * Register method
     *
     * @return \Cake\Http\Response|null|void Renders view
     */
    public function register()
    {
        if ($this->Authentication->getIdentity()) {
            $auth = $this->Authentication->getIdentity()->getOriginalData();
            $user = $this->Users->findById($auth->id)->first();

            if ($user->office_id) {
                return $this->redirect(['controller' => 'President', 'action' => 'edit']);
            }
        }
        $business = $this->Businesses->newEmptyEntity();
        $office = $this->Offices->newEmptyEntity();
        if ($this->request->is('post')) {
            $business = $this->Businesses->patchEntity($business, $this->request->getData()['business']);
            $office = $this->Offices->patchEntity($office, $this->request->getData()['office']);
            if (empty($business->getErrors())) {
                // begin transaction
                $conn = ConnectionManager::get('default');
                $conn->begin();
                try {
                    $conn->transactional(function ($conn) use ($business, $office) {
                        if ($this->Businesses->saveOrFail($business)) {
                            $office->business_id = $business->id;
                            $office->office_type = OFFICE_PRODUCER;
                            $profileImage1 = $this->request->getData('office.file_upload_1');
                            $profileImage2 = $this->request->getData('office.file_upload_2');
                            if (!empty($profileImage1->getSize())) {
                                $dataProfileImage1 = $this->uploadService->uploadAndConvertToPNG($profileImage1, OFFICE_PRODUCER);
                                $office->profile_image_1 = $dataProfileImage1['path'];
                            }
                            if (!empty($profileImage2->getSize())) {
                                $dataProfileImage2 = $this->uploadService->uploadAndConvertToPNG($profileImage2, OFFICE_PRODUCER);
                                $office->profile_image_2 = $dataProfileImage2['path'];
                            }
                            $this->Offices->saveOrFail($office);

                            // Update user office
                            if ($this->Authentication->getIdentity()) {
                                $user = $this->Authentication->getIdentity()->getOriginalData();
                                $user->office_id = $office->id;
                                $this->Users->save($user);
                            }

                            $conn->commit();

                            $this->Flash->success(REGISTER_PRESIDENT_SUCCESS);
                            return $this->redirect(['controller' => 'Farm', 'action' => 'index']);
                        }
                    });
                } catch (\Cake\ORM\Exception\PersistenceFailedException $e) {
                    $conn->rollback();
                }
            }
        }
        $this->set('business', $business);
        $this->set('office', $office);
    }

    /**
     * Edit method
     *
     * @return \Cake\Http\Response|null|void Renders view
     */
    public function edit()
    {
        $auth = $this->Authentication->getIdentity()->getOriginalData();
        $user = $this->Users->findById($auth->id)->first();

        $office = $this->Offices->get($user->office_id);
        $business = $this->Businesses->get($office->business_id);
        if ($this->request->is('post') || $this->request->is('put')) {
            $business = $this->Businesses->patchEntity($business, $this->request->getData()['business']);
            $office = $this->Offices->patchEntity($office, $this->request->getData()['office']);
            if (!$business->getErrors()) {
                $conn = ConnectionManager::get('default');
                $conn->begin();
                try {
                    $conn->transactional(function ($conn) use ($business, $office) {
                        if ($this->Businesses->saveOrFail($business)) {
                            $profileImage1 = $this->request->getData('office.file_upload_1');
                            $profileImage2 = $this->request->getData('office.file_upload_2');
                            if (!empty($profileImage1->getSize())) {
                                $oldImage1 = $office->profile_image_1;
                                $dataProfileImage1 = $this->uploadService->uploadAndConvertToPNG($profileImage1, OFFICE_PRODUCER);
                                $office->profile_image_1 = $dataProfileImage1['path'];
                                $this->uploadService->destroyFile($oldImage1);
                            }
                            if (!empty($profileImage2->getSize())) {
                                $oldImage2 = $office->profile_image_2;
                                $dataProfileImage2 = $this->uploadService->uploadAndConvertToPNG($profileImage2, OFFICE_PRODUCER);
                                $office->profile_image_2 = $dataProfileImage2['path'];
                                $this->uploadService->destroyFile($oldImage2);
                            }
                            $this->Offices->saveOrFail($office);
                            $conn->commit();
                            $this->Flash->success(EDIT_PRESIDENT_SUCCESS);
                            return $this->redirect(['action' => 'edit']);
                        }
                    });
                } catch (\Cake\ORM\Exception\PersistenceFailedException $e) {
                    $conn->rollback();
                }
            }
        }
        $this->set('business', $business);
        $this->set('office', $office);
    }
}
