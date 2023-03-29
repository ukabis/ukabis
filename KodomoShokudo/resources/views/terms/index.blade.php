@extends('layouts.app')

@section('content')
<div class="container">
    <div class="card card_login shadow-sm">
        <div class="card-body card_login-body">
            <div class="row justify-content-center">
                <div class="col-md-7 text-center heading-section ftco-animate fadeInUp ftco-animated">
                    <span class="subheading subp-title">ukabisご利用規約</span>
                    <h2>利用規約</h2>
                </div>
            </div>
            <div class="row justify-content-center">
                <div class="my-3 text-center">
                    {{$termsData['TermsText']}}
                </div>
                    <form method="POST" action="{{ route('termsAgreement')}}">
                        @csrf
                        <input type="text" value="{{$termsData['TermsGroupCode']}}" name="TermsGroupCode" hidden>
                        <input type="text" value="{{$termsData['TermsId']}}" name="TermsId" hidden>
                        <div class="text-center">
                            <label class="CheckboxInput"><input class="CheckboxInput-Input" type="checkbox" required><span class="CheckboxInput-DummyInput"></span><span class="CheckboxInput-LabelText">利用規約を確認し同意しました。</span></label>
                            <div><button type="submit" class="btn-ukabis mt-3">次へ</button></div>
                        </div>
                </form>
            </div>
        </div>
    </div>
</div>
@endsection
