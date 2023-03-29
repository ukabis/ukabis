<?php

namespace App\Http\Controllers;

class OCIAPIAuthController extends Controller
{


    // private $AUTHORIZATION;
    private $AUTHORIZATION_REGISTER;
    private $AUTHORIZATION_LOGIN;
    private $OCI_REGISTER_APP_NAME;
    protected $API_ERROR = 'エラーが発生しました。';
    private $OCI_REGISTER_APP_ID;

    public function __construct()
    {
        $OCI_REGISTER_CLIENT_ID = env('OCI_REGISTER_CLIENT_ID');
        $OCI_REGISTER_CLIENT_SECRET = env('OCI_REGISTER_CLIENT_SECRET');
        $this->AUTHORIZATION_REGISTER = 'Basic ' . base64_encode($OCI_REGISTER_CLIENT_ID .  ':' . $OCI_REGISTER_CLIENT_SECRET);

        $OCI_LOGIN_CLIENT_ID = env('OCI_LOGIN_CLIENT_ID');
        $OCI_LOGIN_CLIENT_SECRET = env('OCI_LOGIN_CLIENT_SECRET');
        $this->AUTHORIZATION_LOGIN = 'Basic ' . base64_encode($OCI_LOGIN_CLIENT_ID .  ':' . $OCI_LOGIN_CLIENT_SECRET);

        $this->API_URL = env('OCI_ENDPOINT');

        $this->OCI_REGISTER_APP_NAME =  env('OCI_REGISTER_APP_NAME');
        $this->OCI_REGISTER_APP_ID = env('OCI_REGISTER_APP_ID');
    }


    /**
     * https://docs.oracle.com/cd/E83857_01/paas/identity-cloud/rest-api/op-oauth2-v1-token-post.html
     * @return mixed|string
     */
    function oauth2_v1_token() {
        $api = "/oauth2/v1/token";
        $headers = array(
            'Authorization: ' . $this->AUTHORIZATION_REGISTER,
            'Content-Type: application/x-www-form-urlencoded;charset=UTF-8'
        );
        $data = array(
            'grant_type' => 'client_credentials',
            'scope' => 'urn:opc:idm:__myscopes__'
        );

        return $this->call_api($api, $headers, http_build_query($data));
    }

    function oauth2_v1_token_login() {
        $api = "/oauth2/v1/token";
        $headers = array(
            'Authorization: ' . $this->AUTHORIZATION_LOGIN,
            'Content-Type: application/x-www-form-urlencoded;charset=UTF-8'
        );
        $data = array(
            'grant_type' => 'client_credentials',
            'scope' => 'urn:opc:idm:__myscopes__'
        );

        return $this->call_api($api, $headers, http_build_query($data));
    }

    /**
     * https://docs.oracle.com/cd/E83857_01/paas/identity-cloud/rest-api/op-admin-v1-userpasswordvalidator-post.html
     * @param $token_type
     * @param $access_token
     * @param $password
     * @return array
     */
    function admin_v1_UserPasswordValidator($token, $password, $username, $given_name, $family_name) {
        $api = "/admin/v1/UserPasswordValidator";
        $headers = array(
            'Content-Type: application/json',
            'Authorization: ' . $token['token_type'] . ' ' . $token['access_token']
        );
        $data = array(
            'userName' => $username,
            'givenName' => $given_name,
            'familyName' => $family_name,
            'password' => $password,
            'schemas' => array('urn:ietf:params:scim:schemas:oracle:idcs:UserPasswordValidator')
        );
        return $this->call_api($api, $headers, json_encode($data), 201);
    }

    /**
     * https://docs.oracle.com/cd/E83857_01/paas/identity-cloud/rest-api/op-admin-v1-users-post.html
     * @param $token_type
     * @param $access_token
     * @param $username
     * @param $email
     * @param $familyName
     * @param $givenName
     * @return array
     */
    public function admin_v1_Users($token, $username, $email, $familyName, $givenName)
    {
        $api = "/admin/v1/Users";
        $headers = array(
            'Content-Type: application/scim+json',
            'Authorization: ' . $token['token_type'] . ' ' . $token['access_token']
        );

        $data = array(
            'userName' => $username,
            "name" => [
                    "givenName" => $givenName,
                    "familyName" => $familyName
            ],
            'emails' => [
                [
                    'value' => $email,
                    'type' => 'WORK',
                    'primary' => true,
                    'verified' => true
                ]
            ],
            'schemas' => array('urn:ietf:params:scim:schemas:core:2.0:User'),
            'urn:ietf:params:scim:schemas:oracle:idcs:extension:user:User' => [
                'bypassNotification' => true
            ]
        );

        return $this->call_api($api, $headers, json_encode($data), 201);
    }

    /**
     * https://docs.oracle.com/cd/E83857_01/paas/identity-cloud/rest-api/op-admin-v1-users-id-delete.html
     * @param $token
     * @param $id
     * @return array
     */
    public function admin_v1_Users_Delete($token, $id)
    {
        $api = "/admin/v1/Users/" . $id;
        $headers = array(
            'Content-Type: application/scim+json',
            'Authorization: ' . $token['token_type'] . ' ' . $token['access_token']
        );

        return $this->call_api($api, $headers, null, 204, 'DELETE');
    }


    /**
     * https://docs.oracle.com/cd/E83857_01/paas/identity-cloud/rest-api/op-mfa-v1-users-userguid-factors-post.html
     * @param $token_type
     * @param $access_token
     * @param $userGUID
     * @return mixed|string
     */
    public function mfa_v1_users_userGUID_factors($token_type, $access_token, $userGUID)
    {
        $api = "/mfa/v1/users/$userGUID/factors";
        $headers = array(
            'Content-Type: application/json',
            'Authorization: ' . $token_type . ' ' . $access_token
        );

        $data = array(
            'method' => 'EMAIL',
//            "skipFactorVerification" => "true"
        );
        return $this->call_api($api, $headers, json_encode($data));
    }


    /**
     * @param $token_type
     * @param $access_token
     * @param $userGUID
     * @param $factorId
     * @param $otpCode
     * @param $requestState
     * @return array
     */
    public function mfa_v1_users_userGUID_factors_factorId($token, $userGUID, $factorId, $otpCode, $requestState)
    {
        $api = "/mfa/v1/users/$userGUID/factors/$factorId";

        $headers = array(
            'Content-Type: application/json',
            'Authorization: ' . $token['token_type'] . ' ' . $token['access_token']
        );

        $data = array(
            'otpCode' => $otpCode,
            "requestState" => $requestState
        );

        return $this->call_api($api, $headers, json_encode($data), 200, 'PATCH');

    }


    /**
     * https://docs.oracle.com/cd/E83857_01/paas/identity-cloud/rest-api/op-admin-v1-userpasswordchanger-id-put.html
     * @param $token
     * @param $userGUID
     * @param $password
     * @return array
     */
    public function admin_v1_UserPasswordChanger($token, $userGUID, $password)
    {
        $api = "/admin/v1/UserPasswordChanger/" . $userGUID;

        $headers = array(
            'Content-Type: application/json',
            'Authorization: ' . $token['token_type'] . ' ' . $token['access_token']
        );

        $data = array(
            'password' => $password,
            'bypassNotification' => true,
            'schemas' => array(
                'urn:ietf:params:scim:schemas:oracle:idcs:UserPasswordChanger'
            )
        );

        return $this->call_api($api, $headers, json_encode($data), 200, 'PUT');
    }

    /**
     * https://docs.oracle.com/cd/E83857_01/paas/identity-cloud/rest-api/op-admin-v1-grants-post.html
     * @param $token_type
     * @param $access_token
     * @param $userGUID
     * @return mixed|string
     */
    public function admin_v1_Grants($token, $userGUID)
    {
        $api = "/admin/v1/Grants";

        $headers = array(
            'Content-Type: application/json',
            'Authorization: ' . $token['token_type'] . ' ' . $token['access_token']
        );

        $data = array(
            'app' => array(
                'value' => $this->OCI_REGISTER_APP_ID
            ),
            'grantMechanism' => 'ADMINISTRATOR_TO_USER',
            'grantee' => array(
                'value' => $userGUID,
                'type' => 'User'
            ),
            'schemas' => array(
                'urn:ietf:params:scim:schemas:oracle:idcs:Grant'
            )
        );

        return $this->call_api($api, $headers, json_encode($data), 201);
    }

    /**
     * https://docs.oracle.com/cd/E83857_01/paas/identity-cloud/rest-api/op-sso-v1-sdk-authenticate-get.html
     * @param $token
     * @return array
     */
    public function authenticate_with_get($token)
    {
        $api = "/sso/v1/sdk/authenticate?appName=" . urlencode($this->OCI_REGISTER_APP_NAME);
        $headers = array(
            "Content-Type: application/json",
            'Authorization: ' . $token['token_type'] . ' ' . $token['access_token']
        );
        return $this->call_api($api, $headers, null, 200, 'GET');
    }

    /**
     * https://docs.oracle.com/cd/E83857_01/paas/identity-cloud/rest-api/op-sso-v1-sdk-authenticate-post.html
     * @param $token
     * @param $username
     * @param $password
     * @param $requestState
     * @return array
     */
    public function authenticate_with_post($token, $username, $password, $requestState)
    {

        $api = "/sso/v1/sdk/authenticate";

        $headers = array(
            'Content-Type: application/json',
            'Authorization: ' . $token['token_type'] . ' ' . $token['access_token']
        );

        $data = array(
            'op' => 'credSubmit',
            "credentials" => [
                "username" => $username,
                "password" => $password
            ],
            'requestState' => $requestState,
        );

        return $this->call_api($api, $headers, json_encode($data));
    }

    /**
     * https://docs.oracle.com/cd/E83857_01/paas/identity-cloud/rest-api/op-oauth2-v1-token-post.html
     * @param $assertion
     * @return array
     */
    public function oauth2_v1_token_with_assertion($assertion)
    {
        $api = "/oauth2/v1/token";
        $headers = array(
            'Content-Type: application/x-www-form-urlencoded',
            'Authorization: ' . $this->AUTHORIZATION_LOGIN,
        );

        $grantType = 'urn:ietf:params:oauth:grant-type:jwt-bearer';
        $scope = 'urn:opc:idm:__myscopes__';

        $data = "grant_type={$grantType}&scope={$scope}&assertion={$assertion}";
        return $this->call_api($api, $headers, $data, 200, 'GET');
    }


    public function admin_v1_Me($token)
    {
        $api = "/admin/v1/Me";
        $headers = array(
            'Content-Type: application/json',
            'Authorization: ' . $token['token_type'] . ' ' . $token['access_token']
        );

        return $this->call_api($api, $headers, null, 200, 'GET');
    }



}
