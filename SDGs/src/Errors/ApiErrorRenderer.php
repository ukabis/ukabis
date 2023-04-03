<?php

namespace App\Errors;

use Cake\Error\ErrorHandler;
use Cake\Http\Response;
use Throwable;

class ApiErrorRenderer extends ErrorHandler
{
    /** @var mixed|null  */
    private $data;

    /** @var Response  */
    private $response;

    /**
     * ApiException constructor
     *
     * @param int $httpCode
     * @param string $message
     * @param null $data
     * @param Throwable|null $previous
     */
    public function __construct(int $httpCode, string $message, $data = null, Throwable $previous = null)
    {
        $this->data = $data;
        parent::__construct($message, $httpCode, $previous);
    }

    /**
     * @return mixed|null
     */
    public function getData()
    {
        return $this->data;
    }

    /**
     * @param string $message
     * @param int $httpCode
     * @return Response
     */
    public static function prepareResponse(int $httpCode, string $message)
    {
        $response = new Response();

        return $response->withType('application/json')
            ->withStatus($httpCode)
            ->withStringBody(json_encode([
                'success' => false,
                'message' => $message
            ]));
    }
}
