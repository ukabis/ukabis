<?php

namespace App\Http\Controllers;
use App\Models\Food;
use Illuminate\Http\Request;
use Illuminate\Pagination\LengthAwarePaginator;


class FoodController extends UkabisAPIController
{
    private $food_field = [
        'foodId',
        'foodName',
        'foodDetail',
        'ingredients',
        'allergen',
        'nutritionInformation',
        'preservationMethod',
        'foodUrl',
        'expirationDate',
        'bestBeforeDate',
        'donorId',
        'donationAmount',
        'donationUnit',
        'closingDateStart',
        'closingDateEnd',
        'minimumRequestAmount',
        'transportationMethod',
        'matchingState',
        'imageFileId',
        'thumbnailFileId',
    ];

    public function create(Request $request){
        $food = new Food();
        return $this->create_and_edit($request, $food);
    }


    /**
     * @param $foodId
     * @param Request $request
     * @return \Illuminate\Contracts\View\Factory|\Illuminate\Contracts\View\View
     */
    public function edit($foodId, Request $request)
    {
        $token = $this->oauth2_v1_token();
        if($token['status']) {
            $vendorToken = $this->getToken();
            if ($vendorToken['status']) {
                $foodData = $this->getFoodInfo($token['data'], $vendorToken['data']['access_token'], $foodId);
                $food = new Food();
                foreach ($this->food_field as $field){
                    $food->$field = $foodData['data'][$field];
                }
                return $this->create_and_edit($request, $food);
            }
        }
    }


    /**
     * @param Request $request
     * @param Food $food
     * @return \Illuminate\Contracts\View\Factory|\Illuminate\Contracts\View\View
     */
    public function create_and_edit(Request $request, Food $food)
    {
        if($request->method() == 'GET'){
            return $this->view($food);
        }else{
            if ($request->has('imageFile1') && $request->has('imageFile2')) {
                // 両方のファイルが存在する場合の処理
                $data = [
                    "imageFile1" =>$request->file('imageFile1'),
                    "imageFile2" => $request->file('imageFile2')
                ];
                $imageDate = array('');

                //TODO API確認
                // $imageDate = $this->call_api($api, $headers, json_encode($data), 200, 'GET');

            } elseif ($request->has('imageFile1')) {
                // $imageFile1だけが存在する場合の処理
                $data = [
                    "imageFile1" =>$request->file('imageFile1'),
                    "imageFile2" =>""
                ];
                $imageDate = array('');
                //TODO API確認
                // $imageDate = $this->call_api($api, $headers, json_encode($data), 200, 'GET');

            } elseif ($request->has('imageFile2')) {
                // $imageFile2だけが存在する場合の処理
                $data = [
                    "imageFile1" =>$request->file('imageFile2'),
                    "imageFile2" =>""
                ];
                $imageDate = array('');
                //TODO API確認
                // $imageDate = $this->call_api($api, $headers, json_encode($imageDate), 200, 'GET');

            } else {
                // 両方とも存在しない場合の処理
                $imageDate = array('');
            }
            if ($request->has('thumbnailFilrId')){
                $thumbnailData = $request->file('thumbnailFilrId');
            }else{
                $thumbnailData = "";
            }

            $token = $this->oauth2_v1_token();
            if($token['status']){
                $vendorToken = $this->getToken();
                if($vendorToken['status']){
                    $foodData = [
                        'foodId' => (string) $request->input('foodId'),
                        'foodName' => (string) $request->input('foodName'),
                        'foodDetail' => (string) $request->input('foodDetail'),
                        'ingredients' => (string) $request->input('ingredients'),
                        'allergen' => (string) $request->input('allergen'),
                        'nutritionInformation' => (string) $request->input('nutritionInformation'),
                        'preservationMethod' => (int) $request->input('preservationMethod'),
                        'foodUrl' => $request->input('foodUrl'),
                        'expirationDate' => $request->input('expirationDate'),
                        'bestBeforeDate' => $request->input('bestBeforeDate'),
                        'donorId' => $request->input('donorId'),
                        'donationAmount' => (int) $request->input('donationAmount'),
                        'donationUnit' => $request->input('donationUnit'),
                        'closingDateStart' => $request->input('closingDateStart'),
                        'closingDateEnd' => $request->input('closingDateEnd'),
                        'minimumRequestAmount' => (int) $request->input('minimumRequestAmount'),
                        'transportationMethod' => implode(',', $request->input('transportationMethod')),
                        'matchingState' => (int) $request->input('matchingState'),
                        'imageFileId' => $imageDate,
                        'thumbnailFileId' => $thumbnailData
                    ];

                    if($foodData['foodId']){
                        $registerFood = $this->updateFood($token['data'], $vendorToken['data']['access_token'], $foodData);
                        if($registerFood['status']){
                            return redirect('/partners/food')->with('success', '編集が完了しました');
                        }else{
                            return redirect('/partners/food')->with('fail', 'ukabis通信エラーが発生しました。もう一度やり直してください。');
                        }
                    }else{
                        $registerFood = $this->registerFood($token['data'], $vendorToken['data']['access_token'], $foodData);
                        if($registerFood['status']){
                            return redirect('/partners/food')->with('success', '登録が完了しました');
                        }else{
                            return view('/food/create')->with('fail', 'ukabis通信エラーが発生しました。もう一度やり直してください。');
                        }
                    }
                }else{
                    return view('/food/create')->with('fail', 'ukabis通信エラーが発生しました。');
                }
            }else{
                return view('/food/create')->with('fail', 'サーバー通信エラーが発生しました。');
            }

        }

    }


    public function foodController (){
        $resource = "/API/Foods";
        // 添付画像ファイルIDの作成
        // /CreateAttachFile
    }

    public function delete (Request $request){
        $token = $this->oauth2_v1_token();
        if($token['status']){
            $vendorToken = $this->getToken();
            if($vendorToken['status']){
                $foodId = $request->input('foodId');
                $deleteFoodItem = $this->deleteFood($token['data'], $vendorToken['data']['access_token'], $foodId);
                if($deleteFoodItem['status']){
                    return redirect('/partners/food')->with('success', '削除しました。');
                }
                return redirect('/partners/food')->with('fail', '削除できませんでした。');
            }else{
                return redirect('/partners/food')->with('fail', 'ukabis通信エラーが発生しました。');
            }
        }else{
            return redirect('/partners/food')->with('fail', 'サーバー通信エラーが発生しました。');
        }
    }

    public function foodsList (Request $request){
        $user = $this->get_user();
        if(!$user){
            return redirect('/partners/login');
        }
        $foodsList = [];
        $token = $this->oauth2_v1_token();
        if($token['status']){
            $vendorToken = $this->getToken();
            if($vendorToken['status']){
                $filter = $request->input('filter');
                $perPage = 10;
                $currentPage = $request->input('page', 1);
                if($filter) {//検索の場合
                    $foodsList = $this->getListByFilter($token['data'], $vendorToken['data']['access_token'], $filter);
                    if(!$foodsList['status']){
                        return view('/food/index', compact('foodsList', 'filter'))->with('fail', $filter.'の検索結果はありませんでした');
                    }else{
                        $foodsList['data'] = $this->paginateApiData($foodsList['data'], $perPage, $currentPage);
                        return view('/food/index', compact('foodsList', 'filter'))->with('success', $filter.'の検索結果です');
                    }
                }else {//一覧の場合
                    $foodsList = $this->getFoods($token['data'], $vendorToken['data']['access_token']);
                    if(!$foodsList['status']){
                        return view('/food/index', compact('foodsList', 'filter'))->with('fail', 'APIエラーにより情報が取得できませんでした');
                    }else{
                        $foodsList['data'] = $this->paginateApiData($foodsList['data'], $perPage, $currentPage);
                        return view('/food/index', compact('foodsList', 'filter'));
                    }
                }
            }else{
                $foodsList['data']= [];
                return view('/food/index', compact('foodsList', 'filter'))->with('fail', 'ukabis API通信エラーが発生しました');
            }
        }else{
            $foodsList['data']= [];
            return view('/food/index', compact('foodsList', 'filter'))->with('fail', 'サーバーAPI通信エラーが発生しました');
        }
    }

    private function paginateApiData($data, $perPage, $currentPage)
    {
        $offset = ($currentPage - 1) * $perPage;
        $items = array_slice($data, $offset, $perPage);

        return new LengthAwarePaginator(
            $items,
            count($data),
            $perPage,
            $currentPage,
            ['path' => LengthAwarePaginator::resolveCurrentPath()]
        );
    }


    /**
     * @param Food $food
     * @param $donorsList
     * @return \Illuminate\Contracts\View\Factory|\Illuminate\Contracts\View\View|\Illuminate\Http\RedirectResponse|\Illuminate\Routing\Redirector
     */
    private function view(Food $food)
    {
        $user = $this->get_user();
        if (!$user) {
            return redirect('/partners/login');
        }

        $token = $this->oauth2_v1_token();
        if ($token['status']) {

            $vendorToken = $this->getToken();
            if ($vendorToken['status']) {
                $donorsList = $this->getList($token['data'], $vendorToken['data']['access_token']);
                if ($donorsList['status']) {
                    return view('/food/create', compact('donorsList', 'food'));
                } else {
                    $donorsList['data'][0] = ['donorId' => "提供者を取得できませんでした"];
                    return view('/food/create', compact('donorsList', 'food'))->with('fail', 'ukabis通信エラーが発生しました。食材を登録するにはリロードしてください。');
                }
            } else {
                $donorsList['data'] = null;
                return view('/food/create', compact('donorsList', 'food'))->with('fail', 'ukabis APIエラー通信エラーが発生しました。食材を登録するにはリロードしてください。');
            }
        } else {
            $donorsList['data'] = null;
            return view('/food/create', compact('donorsList', 'food'))->with('fail', 'サーバー通信エラーが発生しました。食材を登録するにはリロードしてください。');
        }
    }


}




