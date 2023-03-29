@extends('layouts.app')

@section('content')
<div class="container">
    <div class="row justify-content-center">
        <div class="col-md-5">
            <div class="card card_login shadow-sm">
                <div class="card-body card_login-body">
                    <div class="login_header">
                        <p class="login_header__image"><img src="{{ asset('images/logo.png') }}" alt="ukabis"></p>
                        <div class="login_header__title text-center">こども食堂食材提供事業者ログイン</div>
                    </div>
                    <form method="POST">
                        @csrf
                        <div class="mb-3">
                            <div class="">
                                <label for="email" class="col-form-label text-md-end">メールアドレス</label>
                                <input id="email" type="text" class="form-control @error('email') is-invalid @enderror" name="email" value="{{ old('email') }}" required autocomplete="email" autofocus>
                            </div>
                        </div>

                        <div class="mb-3">
                            <div class="">
                                <label for="password" class="col-form-label text-md-end">パスワード</label>
                                <input id="password" type="password" class="form-control @error('password') is-invalid @enderror" name="password" required autocomplete="current-password">
                            </div>
                        </div>

                        @if(isset($error_message))
                            <div class="alert alert-danger">{{ $error_message }}</div>
                        @endif

                        <div class="row mb-0">
                            <div class="text-center my-3">
                                <button type="submit" class="btn-ukabis">
                                    ログイン
                                </button>
                            </div>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </div>
</div>
@endsection
