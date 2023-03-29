<?php

namespace App\Http\Controllers;

use App\Models\User;
use Illuminate\Foundation\Auth\Access\AuthorizesRequests;
use Illuminate\Foundation\Bus\DispatchesJobs;
use Illuminate\Foundation\Validation\ValidatesRequests;
use Illuminate\Routing\Controller as BaseController;
use Illuminate\Support\Facades\Log;
use Illuminate\Support\Facades\Session;
use Psr\Log\LogLevel;

class Controller extends BaseController
{
    use AuthorizesRequests, DispatchesJobs, ValidatesRequests;

    protected $API_URL;
    protected $IS_OCI_API_TYPE = true;

    public function __construct()
    {

    }


    public function get_user() {
        return Session::get('__user');
    }
    public function set_user(User $user) {
        Session::put('__user', $user);
    }
    public function forget_user() {
        Session::forget('__user');
    }

    /**
     * @param $api
     * @param $header
     * @param $data
     * @param int $success_code
     * @param string $method
     * @return array
     */
    protected function call_api($api, $header, $data, $success_code = 200, $method = 'POST'): array
    {
        if (strpos($api, "https://") !== 0) {
            $api = $this->API_URL . $api;
        }

        $ch = curl_init();
        curl_setopt($ch, CURLOPT_URL, $api);
        if($method == 'POST'){
            curl_setopt($ch, CURLOPT_POST, true);
        }else{
            if($method !== 'GET'){
                //PATCH, DELETE
                curl_setopt($ch, CURLOPT_CUSTOMREQUEST, $method);
            }
        }
        if($data){
            curl_setopt($ch, CURLOPT_POSTFIELDS, $data);
        }
        curl_setopt($ch, CURLOPT_HTTPHEADER, $header);
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
        $response = curl_exec($ch);
        $http_code = curl_getinfo($ch, CURLINFO_HTTP_CODE);
        curl_close($ch);
        $result = json_decode($response, true);
        if(isset($result['status'])){
            $result['status'] = $result['status'] == 'success';
        }
        if(!$result){
            $result = [];
        }
        if($this->IS_OCI_API_TYPE){
            $result =  array_merge([
                'status' => $http_code == $success_code,
                'http_code' => $http_code,
            ], $result);
        }else{
            $result =  [
                'status' => $http_code == $success_code,
                'http_code' => $http_code,
                'data' => $result,
            ];
        }
        return $result;
    }
}
