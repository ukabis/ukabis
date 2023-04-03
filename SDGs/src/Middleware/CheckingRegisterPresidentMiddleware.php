<?php

namespace App\Middleware;


use App\Model\Entity\User;
use App\Model\Table\UsersTable;
use Authentication\AuthenticationService;
use Authentication\AuthenticationServiceInterface;
use Cake\ORM\TableRegistry;
use Cake\Routing\Router;
use Laminas\Diactoros\Response\RedirectResponse;
use Psr\Http\Message\ResponseInterface;
use Psr\Http\Message\ServerRequestInterface;
use Psr\Http\Server\RequestHandlerInterface;

class CheckingRegisterPresidentMiddleware implements \Psr\Http\Server\MiddlewareInterface
{

    /**
     * Authentication Service
     */
    protected $_authenticationService;

    /**
     * @var UsersTable
     */
    protected $Users;

    /**
     * Constructor
     *
     * @param AuthenticationService $authenticationService Authentication service instance.
     */
    public function __construct()
    {
        $this->_authenticationService = new AuthenticationService();
        $this->Users = new UsersTable();
    }

    /**
     * Callable implementation for the middleware stack.
     *
     * @param \Psr\Http\Message\ServerRequestInterface $request The request.
     * @param \Psr\Http\Server\RequestHandlerInterface $handler The request handler.
     * @return \Psr\Http\Message\ResponseInterface A response.
     */
    public function process(ServerRequestInterface $request, RequestHandlerInterface $handler): ResponseInterface
    {
        $sessionAuthenticator = $request->getParam('prefix') ?? '';

        $this->_authenticationService->loadAuthenticator('Authentication.Session', [
            'sessionKey' => 'Auth.' . $sessionAuthenticator
        ]);
        $auth = $this->_authenticationService->authenticate($request);

        if (!empty($auth->getData())) {
            $user = $this->Users->findById($auth->getData()->id)->contain('Offices')->first();
            if (empty($user->office)) {
                $url = '';
                switch ($user->role) {
                    case ROLE_RESTAURANT:
                        $url = Router::url(['_name' => 'restaurants:presidentRegister']);
                        break;

                    case ROLE_FARMER:
                        $url = Router::url(['_name' => 'farms:presidentRegister']);
                        break;

                    /**
                     * Continuing implement Checker and Admin role below
                     */

                    default:
                        $url = '/';
                        break;
                }

                return new RedirectResponse($url);
            }
        }

        return $handler->handle($request);
    }
}
