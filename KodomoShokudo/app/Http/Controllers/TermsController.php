<?php

namespace App\Http\Controllers;
use Illuminate\Http\Request;

class TermsController extends UkabisAPIController
{
    public function terms(){
        $token = $this->oauth2_v1_token();
        if($token['status']){
            $vendorToken = $this->getToken();
            if($vendorToken['status']){
                $termsGroupCode = $this->getTermsGroupCode($token['data'], $vendorToken['data']['access_token']);
                if($termsGroupCode['status']){
                    $terms = $this ->get_terms($token['data'], $vendorToken['data']['access_token'], $termsGroupCode['data']['TermsGroupCode']);
                    if($terms['status']){
                        $termsData = $terms['data'][0];
                        // $terms['data']['TermsText'] = 'ukabis利用規約が未設定のため表示できません';
                        session()->forget('fail');
                        return view('/terms/index', compact('termsData'));
                    }else{
                        $termsData['data']['TermsText'] = 'ukabis利用規約が未設定のため表示できません';
                        return view('/terms/index', compact('termsData'));
                    }
                }else{
                    // $terms = $this ->get_terms($token, $vendorToken['access_token'], 'traceability');
                    $termsData['data']['TermsText'] = 'ukabis利用規約が未設定のため表示できません';
                    return view('/terms/index', compact('termsData'));
                }

            }else{
                $termsData['data']['TermsText'] = '通信エラーのため読み込みできません';
                session()->flash('fail', 'APIエラー');
                return view('/terms/index', compact('termsData'));
            }
        }else{
            $termsData['data']['TermsText'] = '通信エラーのため読み込みできません';
            session()->flash('fail', 'APIエラー');
            return view('/terms/index', compact('termsData'));
        }

    }

    public function termsAgreement(Request $request){
        $token = $this->oauth2_v1_token();
        if($token['status']){
            $vendorToken = $this->getToken();
            if($vendorToken['status']){
                $TermsGroupCode = $request->input('TermsGroupCode');
                $TermsId = $request->input('TermsId');
                $termsAgreement = $this->agreementTermsId($token['data'], $vendorToken['data']['access_token'], $TermsId);
                if (!$termsAgreement['status']) {
                    $result = $termsAgreement['data']['title'];
                    session()->flash('success', $result);
                    return redirect('/partners/food');
                }else{
                    // $termsAgreement = $this->agreementByTermsGroupCode($token['data'], $vendorToken['data']['access_token'], $TermsGroupCode);
                    // session()->forget('fail');
                    session()->flash('success', '同意しました');
                    return redirect('/partners/food');
                }
            }else{
                session()->flash('fail', '通信エラーのため同意できませんでした。');
                return redirect('/terms/index');
            }
        }else{
            session()->flash('fail', '通信エラーのため同意できませんでした。');
            return redirect('/terms/index');
        }
    }

}




