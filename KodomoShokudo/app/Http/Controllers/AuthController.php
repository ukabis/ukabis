<?php

namespace App\Http\Controllers;


use App\Common\Utility;
use App\Models\User;
use Carbon\Carbon;
use Illuminate\Contracts\Encryption\DecryptException;
use Illuminate\Http\Request;
use Illuminate\Support\Arr;

class AuthController extends OCIAPIAuthController
{
    public function login(Request $request)
    {
        $user = $this->get_user();
        if($user){
            return redirect('/partners/food');
        }
        if($request->method() == 'GET'){
            return view('/auth/login');
        }else{
            $email = trim($request->input('email'));
            $password = trim($request->input('password'));
            if(!$email || !$password){
                return view('/auth/login')->with('error_message', 'ユーザー名またはパスワードが入力されていません。');
            }

            $token = $this->oauth2_v1_token_login();
            if($token['status']){
                $authenticate_get = $this->authenticate_with_get($token);
                if($authenticate_get['status']){
                    $authenticate_post = $this->authenticate_with_post($token, $email, $password, $authenticate_get['requestState']);
                    if($authenticate_post['status']) {
                        $token_with_assertion = $this->oauth2_v1_token_with_assertion($authenticate_post['authnToken']);
                        if($token_with_assertion['status']) {
                            $admin_v1_Me = $this->admin_v1_Me($token_with_assertion);
                            if($admin_v1_Me['status']){
                                $user = new User();
                                $user->id = $admin_v1_Me['id'];
                                $user->displayName = $admin_v1_Me['displayName'];
                                $user->userName = $admin_v1_Me['userName'];
                                $user->givenName = $admin_v1_Me['name']['givenName'];
                                $user->familyName = $admin_v1_Me['name']['familyName'];
                                $this->set_user($user);
                                return redirect('/partners/terms');
                            }
                        }
                    }else{
                        if(Arr::get($authenticate_post, 'cause.0.code'))
                        {
                            if ($authenticate_post['cause'][0]['code'] == 'AUTH-3004' || $authenticate_post['cause'][0]['code'] == 'AUTH-3005') {
                                //パスワードを変更する必要があります。
                                return redirect('/partners/change_pass')->with('change_pass_token', encrypt($authenticate_post['userId'] . '|' . Carbon::now()->timestamp));
                            }
                            if(Arr::get($authenticate_post, 'cause.0.message')){
                                return view('/auth/login')->with('error_message', Utility::translate_to_jp($authenticate_post['cause'][0]['message']));
                            }
                        }
                    }
                }else{
                    if(Arr::get($authenticate_get, 'cause.0.message')){
                        return view('/auth/login')->with('error_message', Utility::translate_to_jp(Arr::get($authenticate_get, 'cause.0.message')));
                    }
                }
            }
            return view('/auth/login')->with('error_message', $this->API_ERROR);
        }
    }


    public function change_pass(Request $request)
    {
        if($request->method() == 'GET'){
            $change_pass_token = session()->get('change_pass_token');
            if($change_pass_token){
                return view('/auth/change_pass', compact('change_pass_token'));
            }
            return redirect('/partners/login');
        }else{
            try{
                $change_pass_token = $request->input('change_pass_token');
                $token = explode('|', decrypt($change_pass_token));
                $userId = $token[0];
            }catch (DecryptException $ex){
                return redirect('/partners/login');
            }
            if(Carbon::createFromTimestamp($token[1])->addMinutes(30) <= Carbon::now()) {
                return redirect('/partners/login');
            }

            $password = trim($request->input('password'));
            $password_confirmation = trim($request->input('password_confirmation'));
            if(!$password || !$password_confirmation){
                return view('/auth/change_pass', compact('change_pass_token'))->with('error_message', 'パスワードまたはパスワードの確認が入力していません。');
            }
            if($password != $password_confirmation){
                return view('/auth/change_pass', compact('change_pass_token'))->with('error_message', 'パスワードとパスワードの確認が一致しません');
            }

            $token = $this->oauth2_v1_token_login();
            if($token['status']){
                $passwordChanger = $this->admin_v1_UserPasswordChanger($token, $userId, $password);
                if($passwordChanger['status']){
                    return redirect('/partners/login');
                }else {
                    if ($passwordChanger['http_code'] == 400) {
                        return view('/auth/change_pass', compact('change_pass_token'))->with('error_message', $passwordChanger['detail']);
                    }
                }
            }
            return view('/auth/change_pass', compact('change_pass_token'))->with('error_message', $this->API_ERROR);
        }
    }

    public function logout(Request $request)
    {
        $this->forget_user();
        return redirect('/partners/login');
    }
}
