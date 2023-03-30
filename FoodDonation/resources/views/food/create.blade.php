@php /** @var \App\Models\Food $food **/ @endphp
@extends('layouts.app')

@section('content')

<div class="container mt-0">
    <div class="row justify-content-center">
        <div class="col-md-10">
            <div class="my-3">
                <a href="/partners/food" class="btn-ukabis JSLoderBtn">食材一覧へ</a>
            </div>
            <div class="card card_login shadow-sm">
                <div class="card-body card_login-body">
                    <div>
                        <h3 class="subp-title">提供する食材の情報を入力してください。</h3>
                        <p>*は必須項目です</p>
                    </div>
                    <form method="POST" action="{{ route('foodCreate') }}" enctype="multipart/form-data" class="food-form" id="">
                        {!! csrf_field() !!}

                        <input type="hidden" name="foodId" value="{{$food->foodId}}">

                        <div class="form-group">
                            <label for="foodName">食材名<span class="required-form">*</span></label>
                            <input required type="text" class="form-control" id="foodName" name="foodName" value="{{$food->foodName}}" maxlength="64" placeholder="食材名：「日清どん兵衛(1ケース12ピース入り)」など 中身の容量が分かる書き方でお願いします。">
                        </div>

                        <div class="form-group">
                            <label for="foodDetail">詳細<span class="required-form">*</span></label>
                            <textarea required class="form-control" id="foodDetail" name="foodDetail"  value="{{$food->foodDetail}}" maxlength="4000" placeholder="詳細・補足の説明をご入力ください。とくになければ「なし」でも可。">{{$food->foodDetail}}</textarea>
                        </div>

                        <div class="form-group">
                            <label for="ingredients">原材料名<span class="required-form">*</span></label>
                            <textarea required class="form-control" id="ingredients" name="ingredients"  value="{{$food->ingredients}}" maxlength="260" placeholder="原材料情報をご入力ください">{{$food->ingredients}}</textarea>
                        </div>

                        <div class="form-group">
                            <label for="allergen">アレルギー<span class="required-form">*</span></label>
                            <textarea required class="form-control" id="allergen" name="allergen"  value="{{$food->allergen}}" maxlength="260" placeholder="アレルギー情報を入力してください。該当しない場合はなしとご入力ください。">{{$food->allergen}}</textarea>
                        </div>

                        <div class="form-group">
                            <label for="nutritionInformation">栄養成分<span class="required-form">*</span></label>
                            <textarea required class="form-control" id="nutritionInformation" name="nutritionInformation"  value="{{$food->nutritionInformation}}" maxlength="260" placeholder="栄養成分をご入力ください">{{$food->nutritionInformation}}</textarea>
                        </div>

                        <div class="form-group">
                            <label for="preservationMethod">保存方法<span class="required-form">*</span></label>
                            <select required type="radio" class="form-control" id="preservationMethod" name="preservationMethod">
                                @foreach(App\Common\Constant::PRESERVATION_METHOD as $key => $value)
                                    <option value="{{$key}}" {{$food->preservationMethod == $key ? 'selected' : ''}}>{{$value}}</option>
                                @endforeach
                            </select>
                        </div>

                        <div class="form-group">
                            <label for="foodUrl">食材のURL</label>
                            <input class="form-control" id="foodUrl" name="foodUrl"  value="{{$food->foodUrl}}" maxlength="2000" placeholder="食材のURL">
                        </div>

                        <div class="form-group row">
                            <p>消費期限か賞味期限のどちらかをご入力ください。<span class="required-form">*</span></p>
                            <div class="col-md-6">
                                <label>消費期限</label>
                                <div class="input-daterange input-group" id="expirationDate">
                                    <input  type="text" class="input-sm form-control" name="expirationDate" value="{{$food->expirationDate}}">
                                </div>
                            </div>
                            <div class="col-md-6">
                                <label>賞味期限</label>
                                <div class="input-daterange input-group" id="bestBeforeDate">
                                    <input  type="text" class="input-sm form-control" name="bestBeforeDate" value="{{$food->bestBeforeDate}}">
                                </div>
                            </div>
                        </div>

                        <div class="form-group">
                            <label for="donorId">提供者</label>
                            <select required type="radio" class="form-control" id="donorId" name="donorId">
                                <option value="">提供者を選択してください</option>
                                @if(isset($donorsList['data']) && $donorsList['data'])
                                @foreach($donorsList['data'] as $donor)
                                    <option value="{{ $donor['donorId'] }}" {{$donor['donorId'] == $food->donorId ? 'selected' : ''}} >{{ isset($donor['donorName']) ? $donor['donorName'] : $donor['donorId'] }}</option>
                                @endforeach
                                @endif
                            </select>
                        </div>

                        <div class="form-group">
                            <div class="row">
                                <div class="col-md-6">
                                    <label>寄付量<span class="required-form">*</span></label>
                                    <input required type="number" class="input-sm form-control" name="donationAmount" value="{{$food->donationAmount}}">
                                </div>
                                <div class="col-md-6">
                                    <label>寄付量の単位<span class="required-form">*</span></label>
                                    <input required type="text" class="input-sm form-control" name="donationUnit" value="{{$food->donationUnit}}" maxlength="24"  placeholder="">
                                </div>
                            </div>
                        </div>

                        <div class="form-group">
                            <label>画像アップロード<span class="required-form">*</span></label>
                            <div class="small">
                                画像は最大2枚登録できますが、 1枚でも問題ありません。 (1枚あたり10MBのサイズまでなら登録可能です。)
                                ※ファイルの形式はPNG,JPG,JPEG,GIFで、サイズは100MB以下にして下さい。
                            </div>
                            <div class="create-file">
                                <label class="create-file__label">
                                    <div class="create-file__inner">
                                        <svg class="create-file__svg" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z"></path></svg>
                                        <p class='lowercase text-sm text-gray-400 group-hover:text-purple-600 pt-1 tracking-wider'>Select a image 1</p>
                                    </div>
                                    <input  type="file" class="form-control c-button create-file__input" id="imageFile1" name="imageFile1" class="hidden" onchange="previewImage(this);" accept=".png, .jpg, .jpeg, .gif"/>
                                    <p class="create-file__preview">
                                        <img id="imagepreview" src="data:image/gif;base64,R0lGODlhAQABAAAAACH5BAEKAAEALAAAAAABAAEAAAICTAEAOw==" style="max-width:100%;">
                                    </p>
                                </label>
                            </div>


                            <div class="create-file">
                                <label class="create-file__label">
                                    <div class="create-file__inner">
                                        <svg class="create-file__svg" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z"></path></svg>
                                        <p class='lowercase text-sm text-gray-400 group-hover:text-purple-600 pt-1 tracking-wider'>Select a image 2</p>
                                    </div>
                                    <input  type="file" class="form-control c-button create-file__input" id="imageFile2" name="imageFile2" class="hidden" onchange="previewImage2(this);" accept=".png, .jpg, .jpeg, .gif"/>
                                    <p class="create-file__preview">
                                        <img id="imagepreview2" src="data:image/gif;base64,R0lGODlhAQABAAAAACH5BAEKAAEALAAAAAABAAEAAAICTAEAOw==" style="max-width:100%;">
                                    </p>
                                </label>
                            </div>
                        </div>

                        <div class="form-group">
                            <label>サムネイル用画像アップロード<span class="required-form">*</span></label>
                            <div class="small">
                                画像は最大1枚登録できます。 (1枚あたり10MBのサイズまでなら登録可能です。)
                                ※ファイルの形式はPNG,JPG,JPEG,GIFで、サイズは100MB以下にして下さい。
                            </div>
                            <div class="create-file">
                                <label class="create-file__label">
                                    <div class="create-file__inner">
                                        <svg class="create-file__svg" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z"></path></svg>
                                        <p class='lowercase text-sm text-gray-400 group-hover:text-purple-600 pt-1 tracking-wider'>Select a thumbnail</p>
                                    </div>
                                    <input  type="file" class="form-control c-button create-file__input" id="thumbnailFilrId" name="thumbnailFilrId" class="hidden" onchange="thumbpreviewImage(this);" accept=".png, .jpg, .jpeg, .gif"/>
                                    <p class="create-file__preview">
                                        <img id="thumbnaipreview" src="data:image/gif;base64,R0lGODlhAQABAAAAACH5BAEKAAEALAAAAAABAAEAAAICTAEAOw==" style="max-width:100%;">
                                    </p>
                                </label>
                            </div>
                        </div>

                        <h4 class="subp-title mt-2">マッチング情報を入力してください。</h4>
                        <div class="form-group">
                            <label>開始日〜締め日<span class="required-form">*</span></label>
                            <div class="small">
                                締め日は、賞味期限に応じてご設定ください。
                            </div>
                            <div class="input-daterange input-group" id="datepicker">
                                <input required type="text" class="input-sm form-control" name="closingDateStart" value="{{$food->closingDateStart}}">
                                <span class="input-group-text">〜</span>
                                <input required type="text" class="input-sm form-control" name="closingDateEnd"  value="{{$food->closingDateEnd}}">
                            </div>
                        </div>

                        <div class="form-group">
                            <label for="minimumRequestAmount">要望申請の最低単位<span class="required-form">*</span></label>
                            <input required type="number" class="form-control" id="minimumRequestAmount" name="minimumRequestAmount"  value="{{$food->minimumRequestAmount}}" maxlength="50" placeholder="1回の要望で申請できる最低限の申請単位を数値で入力します。前頁の「寄付量」と同じ数値を入力してください。">
                        </div>

                        <div class="form-group">
                            <div><label for="transportationMethod">食材運搬方法<span class="required-form">*</span></label></div>
                                @foreach(App\Common\Constant::TRANSPORTATIONMETHOD as $key => $value)
                                    <div class="form-checkbox">
                                        <input type="checkbox" id="transportationMethod{{$key}}" name="transportationMethod[]" value="{{$key}}" {{ str_contains($food->transportationMethod, $key) ? 'checked' : ''}} class="required-check">
                                        <label for="transportationMethod{{$key}}">{{$value}}</label>
                                    </div>
                                @endforeach
                        </div>

                        <div class="form-group">
                            <label for="matchingState">マッチング状況<span class="required-form">*</span></label>
                            <select required type="radio" class="form-control" id="matchingState" name="matchingState">
                                @foreach(App\Common\Constant::MATCHINGSTATE as $key => $value)
                                    <option value="{{$key}}" {{$food->matchingState == $key ? 'selected' : ''}}>{{$value}}</option>
                                @endforeach
                            </select>
                        </div>

                        <div class="text-center mt-3">
                            <button type="submit" class="btn-ukabis mt-3">
                                @if (Str::contains(url()->current(), 'edit'))
                                食材情報を保存する
                                @else
                                食材を登録する
                                @endif
                            </button>
                        </div>

                        <div id="alert-box" class="mt-3"></div>
                    </form>
                </div>
            </div>
        </div>
    </div>
</div>

<script>

    var _validFileExtensions = [".png", ".jpg", ".jpeg", ".gif"];
    function check_file_type_and_size(oInput) {
        if (oInput.type == "file") {
            var sFileName = oInput.value;
            if (sFileName.length > 0) {
                var blnValid = false;
                for (var j = 0; j < _validFileExtensions.length; j++) {
                    var sCurExtension = _validFileExtensions[j];
                    if (sFileName.substr(sFileName.length - sCurExtension.length, sCurExtension.length).toLowerCase() === sCurExtension.toLowerCase()) {
                        blnValid = true;
                        break;
                    }
                }

                if (!blnValid) {
                    alert("ファイルの形式が正しくありません");
                    oInput.value = "";
                    return false;
                }
                if((oInput.files[0].size / 1024 /1024).toFixed(2) > 100){
                    alert("ファイルのサイズが最大サイズを超えています");
                    oInput.value = "";
                    return false;
                }
            }
        }
        return true;
    }

    function previewImage(obj)
    {
        if(check_file_type_and_size(obj) === true){

            var fileReader = new FileReader();
            fileReader.onload = (function() {
                document.getElementById('imagepreview').src = fileReader.result;
            });
            fileReader.readAsDataURL(obj.files[0]);
        }
    }

    function previewImage2(obj)
    {
        if(check_file_type_and_size(obj) === true){

            var fileReader = new FileReader();
            fileReader.onload = (function() {
                document.getElementById('imagepreview2').src = fileReader.result;
            });
            fileReader.readAsDataURL(obj.files[0]);
        }
    }

    function thumbpreviewImage(obj)
    {
        if(check_file_type_and_size(obj) === true){

            var fileReader = new FileReader();
            fileReader.onload = (function() {
                document.getElementById('thumbnaipreview').src = fileReader.result;
            });
            fileReader.readAsDataURL(obj.files[0]);
        }
    }

    $(document).ready(function () {
        $.ajaxSetup({
            headers: {
                'X-CSRF-TOKEN': $('meta[name="csrf-token"]').attr('content')
            }
        });

        $('.input-daterange').datepicker({
            // オプションを設定
            language:'ja', // 日本語化
            format: 'yyyy-mm-dd', // 日付表示をyyyy/mm/ddにフォーマット
        });
});


$(function() {
    // Submitボタンがクリックされたら
    $("form").on('submit', function(event) {
      // チェックされているチェックボックスの数
    let check = false;
    if ($(".required-check:checked").length > 0) {
        check = true;
    }
    if (!check) {
        event.preventDefault(); // フォームの送信を中止する
        $('#alert-box').html('<div class="alert alert-danger alert-dismissible fade show"><div>食材運搬方法を選択してください</div><button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
        }
    });
});


$(function() {
    // Submitボタンがクリックされたら
    $("form").on('submit', function(event) {
    if (!$('input[name="expirationDate"]').val() && !$('input[name="bestBeforeDate"]').val()) {
        event.preventDefault(); // フォームの送信を中止する
        $('#alert-box').html('<div class="alert alert-danger alert-dismissible fade show"><div>消費期限あるいは賞味期限を設定してください</div><button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>');
    }
    });
});
</script>
@endsection
