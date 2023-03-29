<?php

namespace App\Http\Controllers;

use App\Common\Utility;
use Illuminate\Contracts\Encryption\DecryptException;
use Illuminate\Http\Request;

class RegisterController extends OCIAPIAuthController
{
    public function index(Request $request)
    {
        if($request->method() == 'GET'){
            return view('/register/index');
        }else{

            $rules = [
                'email'  => 'required|email',
                'familyName' => 'required',
                'givenName' => 'required',
                'password' => 'required',
            ];

            $messages = [
                'email.required' => 'メールアドレスを入力してください',
                'email.email' => '有効なメールアドレスを入力してください',
                'familyName.required' => 'お名前を入力してください',
                'givenName.required' => 'お名前を入力してください',
                'password.required' => 'パスワードを入力してください',
            ];
            $request->validate($rules, $messages);


            $email = $request->input('email');
            $username = $request->input('email');
            $familyName = $request->input('familyName');
            $givenName = $request->input('givenName');
            $password = $request->input('password');


            $token = $this->oauth2_v1_token();
            if($token['status']){
                $passwordValidator = $this->admin_v1_UserPasswordValidator($token, $password, $username, $givenName, $familyName);
                if($passwordValidator['status']){
                    $create_user = $this->admin_v1_Users($token, $username, $email, $familyName, $givenName);
                    if($create_user['status']){
                        $passwordChanger = $this->admin_v1_UserPasswordChanger($token, $create_user['id'], $password);
                        if($passwordChanger['status']){
                            $factors = $this->mfa_v1_users_userGUID_factors($token['token_type'], $token['access_token'], $create_user['id']);
                            if($factors['status']){
                                return redirect("/partners/register/code?factorid=".
                                    encrypt($factors['factorId']).
                                    '&requeststate='.encrypt($factors['requestState']).
                                    '&userid='.encrypt($create_user['id']));
                            }
                        }else{
                            $admin_v1_Users_Delete = $this->admin_v1_Users_Delete($token, $create_user['id']);
                            if($admin_v1_Users_Delete['status']){
                                return redirect()->back()->withInput()->with('error_message', Utility::translate_to_jp($passwordValidator['failedPasswordPolicyRules'][0]['value']));
                            }
                        }
                    }else{
                        if($create_user['http_code']=='409'){
                            return redirect()->back()->withInput()->with('error_message', 'メールアドレスが登録されています。');
                        }
                    }
                }else{
                    if(isset($passwordValidator['failedPasswordPolicyRules']) &&
                        isset($passwordValidator['failedPasswordPolicyRules'][0]) &&
                        isset($passwordValidator['failedPasswordPolicyRules'][0]['value'])
                    ){
                        return redirect()->back()->withInput()->with('error_message', Utility::translate_to_jp($passwordValidator['failedPasswordPolicyRules'][0]['value']));
                    }
                }
            }
            return redirect()->back()->withInput()->with('error_message', $this->API_ERROR);
        }
    }


    public function code(Request $request){

        if($request->method() == 'GET'){
            return view('/register/code');
        }else{
            $code           = $request->input('code', '');

            try{
                $factorid       = decrypt($request->get('factorid'));
                $userid         = decrypt($request->get('userid'));
                $requeststate   = decrypt($request->get('requeststate'));
            }catch (DecryptException $ex){
                return redirect()->back()->withInput()->with('error_message', $this->API_ERROR);
            }

            $data = [
                'factorid' => $factorid,
                'requeststate' => $requeststate,
                'userid' => $userid,
            ];
            if(!$code){
                return redirect()->back()->withInput()->with('error_message', '有効なコードを入力してください');
            }

            $token = $this->oauth2_v1_token();
            if($token['status']){
                $factor_confirm_code = $this->mfa_v1_users_userGUID_factors_factorId($token, $data['userid'], $data['factorid'], $code, $data['requeststate']);
                if($factor_confirm_code['status']){
                    $grants = $this->admin_v1_Grants($token, $data['userid']);
                    if($grants['status']){
                        return redirect('/partners/login');
                    }
                }else{
                    return redirect()->back()->withInput()->with('error_message', '有効なコードを入力してください');
                }
            }
            return redirect()->back()->withInput()->with('error_message', $this->API_ERROR);
        }
    }
}
