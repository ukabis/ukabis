@extends('layouts.app')

@section('content')
<div class="container">
    <div class="row justify-content-center">
        <div class="col-md-6">
            <div class="card card_login shadow-sm">
                <div class="card-body card_login-body">
                    <div class="login_header">
                        <p class="login_header__image"><img src="{{ asset('images/logo.png') }}" alt="ukabis"></p>
                        <div class="login_header__title text-center">食材提供事業者登録</div>
                    </div>
                    <form method="POST">
                        @csrf

                        <div class="row mb-3">
                            <label for="code" class="col-md-4 col-form-label text-md-end">{{ __('確認コードの入力') }}</label>
                            <div class="col-md-6">
                                <input required id="code" type="text" class="form-control @error('code') is-invalid @enderror" name="code" value="{{ old('code') }}"  autofocus>

                                @if(session('error_message'))
                                    <p class="text-danger" role="alert">
                                        <strong>{{ session('error_message') }}</strong>
                                    </p>
                                @endif
                                <div class="small my-2 form-warning">確認コードの有効期限は10分間です。</div>
                            </div>
                        </div>

                        <div class="row mb-0">
                            <div class="col-md-8 offset-md-4">
                                <button type="submit" class="btn-ukabis">メール認証</button>
                            </div>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </div>
</div>
@endsection
