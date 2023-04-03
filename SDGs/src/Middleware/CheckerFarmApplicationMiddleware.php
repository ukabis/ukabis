<?php
declare(strict_types=1);

namespace App\Middleware;

use App\Model\Table\FarmQuestionsTable;
use Cake\Routing\Router;
use Laminas\Diactoros\Response\RedirectResponse;
use Psr\Http\Message\ResponseInterface;
use Psr\Http\Message\ServerRequestInterface;
use Psr\Http\Server\MiddlewareInterface;
use Psr\Http\Server\RequestHandlerInterface;

/**
 * CheckerQuestion middleware
 */
class CheckerFarmApplicationMiddleware implements MiddlewareInterface
{
    /**
     * @var FarmQuestionsTable
     */
    protected $FarmQuestions;

    /**
     * Constructor
     *
     * @param AuthenticationService $authenticationService Authentication service instance.
     */
    public function __construct()
    {
        $this->FarmQuestions = new FarmQuestionsTable();
    }

    /**
     * Process method.
     *
     * @param \Psr\Http\Message\ServerRequestInterface $request The request.
     * @param \Psr\Http\Server\RequestHandlerInterface $handler The request handler.
     * @return \Psr\Http\Message\ResponseInterface A response.
     */
    public function process(ServerRequestInterface $request, RequestHandlerInterface $handler): ResponseInterface
    {
        $id = $request->getParam('id');
        $flash = $request->getAttribute('flash');
        $farmAnswer = $this->FarmQuestions->findById($id)->firstOrFail();

        if ($farmAnswer->status === APPLICATION_STATUS_ANSWER) {
            $flash->error(NOT_ALLOW_ACCESS_BEFORE_SCORING);

            return new RedirectResponse(Router::url(['controller' => 'FarmApplication', 'action' => 'score', 'id' => $id]));
        }

        return $handler->handle($request);
    }
}
