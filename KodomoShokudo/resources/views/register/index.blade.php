@extends('layouts.app')

@section('content')
<div class="container">
    <div class="row justify-content-center">
        <div class="col-md-8">
            <div class="card card_login shadow-sm">
                <div class="card-body  card_login-body">
                    <div class="login_header">
                        <p class="login_header__image"><img src="{{ asset('images/logo.png') }}" alt="ukabis"></p>
                        <div class="login_header__title text-center">こども食堂食材提供事業者登録</div>
                    </div>
                    <form method="POST" action="{{ route('register.index') }}">
                        @csrf

                        <div class="row mb-3">
                            <label for="familyName" class="col-md-4 col-form-label text-md-end">姓</label>
                            <div class="col-md-6">
                            <input id="familyName" type="text" class="form-control @error('familyName') is-invalid @enderror" name="familyName" value="{{ old('familyName') }}"  autofocus>
                            @error('familyName')
                            <span class="invalid-feedback" role="alert">
                                <strong>{{ $message }}</strong>
                            </span>
                            @enderror
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label for="givenName" class="col-md-4 col-form-label text-md-end">名</label>
                            <div class="col-md-6">
                            <input id="givenName" type="text" class="form-control @error('givenName') is-invalid @enderror" name="givenName" value="{{ old('givenName') }}"  autofocus>
                            @error('givenName')
                            <span class="invalid-feedback" role="alert">
                                <strong>{{ $message }}</strong>
                            </span>
                            @enderror
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label for="email" class="col-md-4 col-form-label text-md-end">メールアドレス</label>
                            <div class="col-md-6">
                                <input id="email" type="" class="form-control @error('email') is-invalid @enderror" name="email" value="{{ old('email') }}"  autofocus>
                                @error('email')
                                <span class="invalid-feedback" role="alert">
                                <strong>{{ $message }}</strong>
                            </span>
                                @enderror
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label for="password" class="col-md-4 col-form-label text-md-end">パスワード</label>
                            <div class="col-md-6">
                            <input id="password" type="password" class="form-control @error('password') is-invalid @enderror" name="password" value="{{ old('password') }}"  autofocus onclick="showWarning()">
                            @error('password')
                            <span class="invalid-feedback" role="alert">
                                <strong>{{ $message }}</strong>
                            </span>
                            @enderror
                            <div class="small my-2 form-warning"  id="password-warning" style="display:none;">
                                *パスワードは8文字以上40文字以内で設定してください。<br>
                                *パスワードには1個以上の大文字、1個以上の小文字、1個以上の数字を設定してください。<br>
                                *ユーザーの姓、名、ユーザーID、空白文字を含めることはできません。
                            </div>
                            </div>
                        </div>

                        <div class="row mb-0">
                            <div class="col-md-8 offset-md-4">
                                @if(session('error_message'))
                                    <p class="text-danger" role="alert">
                                        <strong>{{ session('error_message') }}</strong>
                                    </p>
                                @endif
                                <button type="submit" class="btn-ukabis">メールアドレス認証へ進む</button>
                            </div>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </div>
</div>
@endsection


<script>
function showWarning() {
    document.getElementById("password-warning").style.display = "block";
}
</script>