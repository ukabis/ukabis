<?php

declare(strict_types=1);

namespace App\Middleware;

use App\Model\Table\RestaurantQuestionsTable;
use Cake\Routing\Router;
use Laminas\Diactoros\Response\RedirectResponse;
use Psr\Http\Message\ResponseInterface;
use Psr\Http\Message\ServerRequestInterface;
use Psr\Http\Server\MiddlewareInterface;
use Psr\Http\Server\RequestHandlerInterface;

/**
 * CheckerRestaurantApplication middleware
 */
class CheckerRestaurantApplicationMiddleware implements MiddlewareInterface
{
    /**
     * @var RestaurantQuestionsTable
     */
    protected $restaurantQuestions;

    /**
     * Constructor function
     */
    public function __construct()
    {
        $this->restaurantQuestions = new RestaurantQuestionsTable();
    }

    /**
     * Process method.
     *
     * @param \Psr\Http\Message\ServerRequestInterface $request The request.
     * @param \Psr\Http\Server\RequestHandlerInterface $handler The request handler.
     *
     * @return \Psr\Http\Message\ResponseInterface A response.
     */
    public function process(ServerRequestInterface $request, RequestHandlerInterface $handler): ResponseInterface
    {
        $id = $request->getParam('id');
        $flash = $request->getAttribute('flash');
        $restaurantAnswer = $this->restaurantQuestions->findById($id)->firstOrFail();

        if ($restaurantAnswer->status === APPLICATION_STATUS_ANSWER) {
            $flash->error(NOT_ALLOW_ACCESS_BEFORE_SCORING);

            return new RedirectResponse(Router::url(['controller' => 'RestaurantApplication', 'action' => 'score', 'id' => $id]));
        }

        return $handler->handle($request);
    }
}
