<?php
declare(strict_types=1);

/**
 * CakePHP(tm) : Rapid Development Framework (https://cakephp.org)
 * Copyright (c) Cake Software Foundation, Inc. (https://cakefoundation.org)
 *
 * Licensed under The MIT License
 * For full copyright and license information, please see the LICENSE.txt
 * Redistributions of files must retain the above copyright notice.
 *
 * @copyright Copyright (c) Cake Software Foundation, Inc. (https://cakefoundation.org)
 * @link      https://cakephp.org CakePHP(tm) Project
 * @since     0.2.9
 * @license   https://opensource.org/licenses/mit-license.php MIT License
 */
namespace App\Controller;

use Cake\Controller\Controller;
use Cake\Core\Configure;
use Cake\ORM\Table;

/**
 * Application Controller
 *
 * Add your application-wide methods in the class below, your controllers
 * will inherit them.
 *
 * @link https://book.cakephp.org/4/en/controllers.html#the-app-controller
 */
class AppController extends Controller
{
    /**
     * Initialization hook method.
     *
     * Use this method to add common initialization code like loading components.
     *
     * e.g. `$this->loadComponent('FormProtection');`
     *
     * @return void
     */
    public function initialize(): void
    {
        parent::initialize();

        $this->loadComponent('RequestHandler');
        $this->loadComponent('Flash');
        $this->loadComponent('Authentication.Authentication');
        /*
         * Enable the following component for recommended CakePHP form protection settings.
         * see https://book.cakephp.org/4/en/controllers/components/form-protection.html
         */
        //$this->loadComponent('FormProtection');
        $this->getEnvironment();
    }

    /**
     * Get user information after logging in with relation
     *
     * @param array $associations
     */
    public function getUserLoggedIn(array $associations = [])
    {
        $user = null;
        if ($auth = $this->Authentication->getIdentity()) {
            if ($user = $this->Users->findById($auth->id)->contain($associations)->firstOrFail()) {
                $this->set('user', $user);
            }
        }

        return $user;
    }

    /**
     * Get list requests of Restaurant or Farm
     *
     * @param Table $table
     */
    public function getLatestRequestsOffice(Table $table)
    {
        if ($auth = $this->Authentication->getIdentity()) {
            $user = $this->Users->findById($auth->id)->contain('Offices')->firstOrFail();
            $dataRequests = $table->getLatestRequestByOfficeId($table, $user->office->id, ['comment', 'created_at']);

            $this->set('dataRequest', $dataRequests);
        }
    }

    /**
     * Get app environment
     *
     */
    public function getEnvironment()
    {
        Configure::load('app', 'default');
        $env = Configure::read('app_env');

        $this->set('env', $env);
    }
}
