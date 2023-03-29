@extends('layouts.app')

@section('content')
    <div class="container">
        <div class="row justify-content-center">
            <div class="col-md-4">
                <div class="card card_login shadow-sm">
                    <div class="card-body card_login-body">
                        <div class="login_header">
                            <p class="login_header__image"><img src="{{ asset('images/logo.png') }}" alt="ukabis"></p>
                            <div class="login_header__title text-center">パスワードの変更</div>
                        </div>
                        <form method="POST">
                            @csrf
                            <input type="hidden" name="change_pass_token" value="{{$change_pass_token}}">
                            <div class="mb-3">
                                <div class="">
                                    <label for="password" class="col-form-label text-md-end">新しいパスワード</label>
                                    <input id="password" type="password" class="form-control @error('password') is-invalid @enderror" name="password" required autocomplete="current-password">
                                </div>
                            </div>

                            <div class="mb-3">
                                <div class="">
                                    <label for="password_confirmation" class="col-form-label text-md-end">新しいパスワードの確認</label>
                                    <input id="password_confirmation" type="password" class="form-control @error('password_confirmation') is-invalid @enderror" name="password_confirmation" required autocomplete="current-password_confirmation">
                                </div>
                            </div>

                            @if(isset($error_message))
                                <div class="alert alert-danger">{{ $error_message }}</div>
                            @endif

                            <div class="row mb-0">
                                <div class="text-center my-3">
                                    <button type="submit" class="btn-ukabis">
                                        パスワードを変更する
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
