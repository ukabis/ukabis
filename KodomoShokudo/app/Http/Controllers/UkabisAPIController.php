<?php

namespace App\Http\Controllers;


class UkabisAPIController extends Controller
{
    private $OPEN_ID_ENDOPOINT;
    private $VENDOR_CLIENT_ID;
    private $VENDOR_CLIENT_SECRET;

    private $OPEN_ID_CLIENT_ID;
    private $OPEN_ID_CLIENT_SECRET;
    private $OPEN_ID_USER_NAME;
    private $OPEN_ID_PASS;
    private $OPEN_ID_SCOPE;

    public function __construct()
    {
        $this->API_URL = env('DYNAMIC_API_ENDPOINT');
        $this->OPEN_ID_ENDOPOINT = env('OPEN_ID_ENDOPOINT');

        $this->OPEN_ID_ENDOPOINT = env('OPEN_ID_ENDOPOINT');
        $this->IS_OCI_API_TYPE = false;

        $this->VENDOR_CLIENT_ID = env('VENDOR_CLIENT_ID');
        $this->VENDOR_CLIENT_SECRET = env('VENDOR_CLIENT_SECRET');

        $this->OPEN_ID_CLIENT_ID = env('OPEN_ID_CLIENT_ID');
        $this->OPEN_ID_CLIENT_SECRET = env('OPEN_ID_CLIENT_SECRET');
        $this->OPEN_ID_USER_NAME = env('OPEN_ID_USER_NAME');
        $this->OPEN_ID_PASS = env('OPEN_ID_PASS');

        $this->OPEN_ID_SCOPE = env('OPEN_ID_SCOPE');
    }

    public function getToken()
    {
        $api = '/Token';
        $headers = array(
            'Content-Type: application/x-www-form-urlencoded',
        );
        $data = array(
            'grant_type' => 'client_credentials',
            'client_id' => $this->VENDOR_CLIENT_ID,
            'client_secret' => $this->VENDOR_CLIENT_SECRET
        );
        return $this->call_api($api, $headers, http_build_query($data));
    }


    public function getList($token, $vendorToken)
    {
        $api = "/API/Donation/V2/Private/Donors/GetList";

        $headers = array(
            'X-Authorization: ' .$vendorToken,
            'Authorization: ' .$token['token_type']. ' ' .$token['access_token'],
            'Content-Type: application/x-www-form-urlencoded;charset=UTF-8'
        );

        // ドナーサンプルデータ
        // $donorData = $this->DONORS_JSON;
        // return json_decode($donorData, true);

        // API完成後
        return $this->call_api($api, $headers, null, 200, 'GET');
    }

    public function getFoods($token, $vendorToken)
    {
        $api = "/API/Donation/V2/Private/Foods/GetList";
        $headers = array(
            'X-Authorization: ' .$vendorToken,
            'Authorization: ' .$token['token_type']. ' ' .$token['access_token'],
            'Content-Type: application/x-www-form-urlencoded;charset=UTF-8'
        );
        // フードサンプルデータ
        // $foodsList = $this -> FOODS_JSON;
        // return json_decode($foodsList, true);

        // API後
        return $this->call_api($api, $headers, null, 200, 'GET');
    }

    public function registerFood($token, $vendorToken, $foodData)
    {
        $api = "/API/Donation/V2/Private/Foods/Register";

        $headers = array(
            'X-Authorization: ' .$vendorToken,
            'Authorization: ' .$token['token_type']. ' ' .$token['access_token'],
            'Content-Type: application/x-www-form-urlencoded;charset=UTF-8'
        );
        unset($foodData['foodId']);
        return $this->call_api($api, $headers, json_encode($foodData), 201, 'POST');
    }

    public function deleteFood($token, $vendorToken, $foodId)
    {
        $api = '/API/Donation/V2/Private/Foods/Delete/'.$foodId;

        $headers = array(
            'X-Authorization: ' .$vendorToken,
            'Authorization: ' .$token['token_type']. ' ' .$token['access_token'],
            'Content-Type: application/x-www-form-urlencoded;charset=UTF-8'
        );
        return $this->call_api($api, $headers, $foodId, 204, 'DELETE');
    }

    public function getFoodInfo($token, $vendorToken, $foodId)
    {
        $api = '/API/Donation/V2/Private/Foods/Get/'.$foodId;

        $headers = array(
            'X-Authorization: ' .$vendorToken,
            'Authorization: ' .$token['token_type']. ' ' .$token['access_token'],
            'Content-Type: application/x-www-form-urlencoded;charset=UTF-8'
        );
        return $this->call_api($api, $headers, null, 200, 'GET');
    }

    public function updateFood($token, $vendorToken, $foodData)
    {
        $api = '/API/Donation/V2/Private/Foods/Update/'.$foodData['foodId'];

        $headers = array(
            'X-Authorization: ' .$vendorToken,
            'Authorization: ' .$token['token_type']. ' ' .$token['access_token'],
            'Content-Type: application/x-www-form-urlencoded;charset=UTF-8'
        );
        unset($foodData['foodId']);
        return $this->call_api($api, $headers, json_encode($foodData), 204, 'PATCH');
    }

    // ログオンユーザの規約グループコードを取得する
    public function getTermsGroupCode($token, $vendorToken)
    {
        $api = '/Manage/TermsGroup/GetTermsGroupCode';
        $headers = array(
            'X-Authorization: ' . $vendorToken,
            'Authorization: ' . $token['token_type']. ' ' .$token['access_token'],
            'Content-Type: application/x-www-form-urlencoded;charset=UTF-8'
        );
        return $this->call_api($api, $headers, null, 200, 'GET');
    }

    // 規約グループコードに合致した規約を取得する
    function get_terms($token, $vendorToken, $termsCode){
        $api = '/Manage/Terms/GetListByTermsGroupCode?terms_group_code='.$termsCode;
        $headers = array(
            'X-Authorization: ' . $vendorToken,
            'Authorization: ' . $token['token_type']. ' ' .$token['access_token'],
            'Content-Type: application/x-www-form-urlencoded;charset=UTF-8'
        );
        return $this->call_api($api, $headers, null, 200, 'GET');
    }

    //規約の同意・撤回の履歴を取得する
    function user_terms_getlist($token, $vendorToken){
        $api = '/Manage/UserTerms/GetList';
        $headers = array(
            'X-Authorization: ' . $vendorToken,
            'Authorization: ' . $token['token_type']. ' ' .$token['access_token'],
            'Content-Type: application/x-www-form-urlencoded;charset=UTF-8'
        );
        return $this->call_api($api, $headers, null, 200, 'GET');
    }

    //指定した規約グループコードの規約に同意する
    public function agreementByTermsGroupCode($token, $vendorToken, $TermsGroupCode)
    {
        $api = "/Manage/Terms/AgreementByTermsGroupCode";
        $headers = array(
            'X-Authorization: ' . $vendorToken,
            'Authorization: ' . $token['token_type']. ' ' .$token['access_token'],
            'Content-Type: application/x-www-form-urlencoded;charset=UTF-8'
        );
        $data = array(
            'TermsGroupCode' => $TermsGroupCode
        );
        return $this->call_api($api, $headers, json_encode($data), 200, 'POST');
    }

    //指定した規約IDの規約に同意する
    public function agreementTermsId($token, $vendorToken, $TermsId)
    {
        $api = '/Manage/Terms/Agreement?terms_id='.$TermsId;
        $headers = array(
            'X-Authorization: ' . $vendorToken,
            'Authorization: ' . $token['token_type']. ' ' .$token['access_token'],
            'Content-Type: application/x-www-form-urlencoded;charset=UTF-8'
        );
        return $this->call_api($api, $headers, null, 200, 'POST');
    }

    function oauth2_v1_token() {
        $api =  $this->OPEN_ID_ENDOPOINT . "/oauth2/v1/token";

        $headers = array(
            'Content-Type: application/x-www-form-urlencoded'
        );

        $data = array(
            'client_id' => $this->OPEN_ID_CLIENT_ID,
            'client_secret' => $this->OPEN_ID_CLIENT_SECRET,
            'username' => $this->OPEN_ID_USER_NAME,
            'password' => $this->OPEN_ID_PASS,
            'grant_type' => 'password',
            'scope' => $this->OPEN_ID_SCOPE
        );

        return $this->call_api($api, $headers, http_build_query($data));
    }

    //TODO
    //画像アップロード
    function uploadAttachFile($token, $vendorToken){
        $api = "/API/Donation/V2/Private/Foods/UploadAttachFile";
        $headers = array(
            'X-Authorization: ' . $vendorToken,
            'Authorization: ' . $token['token_type']. ' ' .$token['access_token'],
            'Content-Type: application/x-www-form-urlencoded;charset=UTF-8'
        );
        $data = array(
            //画像ファイル
        );
        return $this->call_api($api, $headers, $data, 200, 'POST');
    }


    /**
     * @param $token
     * @param $vendorToken
     * @param $filter
     * @return array
     */
    function getListByFilter($token, $vendorToken, $filter){
        $encoded_filter = urlencode($filter);
        $api = "/API/Donation/V2/Private/Foods/GetListByFilter?filter=" . rawurlencode("contains(foodName,'$encoded_filter')");

        $headers = array(
            'X-Authorization: ' . $vendorToken,
            'Authorization: ' . $token['token_type']. ' ' .$token['access_token'],
            'Content-Type: application/x-www-form-urlencoded;charset=UTF-8'
        );

        return $this->call_api($api, $headers, null, 200, 'GET');
    }
}
