@extends('layouts.app')

@section('content')
    <div class="album py-2 bg-light">
        <div class="container">
            <div class="d-flex justify-content-between align-items-center my-3">
                <h2 class="h3 subp-title">提供中の食材一覧</h2>
                <a class="btn-ukabis JSLoderBtn" href="/partners/food/create">食材新規追加</a>
            </div>
            <form method="GET" action="{{ route('foodsList') }}">
                <div class="container food-search mt-2 mb-1">
                    <div class="position-relative">
                        <input type="text" class="form-control" placeholder="食材名から検索" name="filter" value="{{ $filter }}">
                    </div>
                    <div class="d-flex justify-content-center align-items-center">
                        <button class="btn btn-secondary text-secondary-emphasis m-1 mt-3" type="submit">　検索　</button>
                    </div>
                </div>
            </form>
            <div class="row">
            <div class="card card_login shadow-sm">
                <div class="card-body card_login-body">
                <table class="table table-hover foodlist-table">
                    <thead>
                    <tr>
                        <th>食材名</th>
                        <th>マッチング開始日</th>
                        <th>マッチング締め日</th>
                        <th>消費期限</th>
                        <th>賞味期限</th>
                        <th>寄付量</th>
                        <th>マッチング状態</th>
                        <th>アクション</th>
                    </tr>
                    </thead>
                    <tbody>
                        @if (empty($foodsList['data']))
                            <tr><td colspan="8">リストを取得できませんでした<td></tr>
                        @else
                        @foreach($foodsList['data'] as $food)
                        <tr>
                            <td>{{ $food['foodName'] }}</td>
                            <td>{{ $food['closingDateStart'] }}</td>
                            <td>{{ $food['closingDateEnd'] }}</td>
                            <td>{{ $food['expirationDate'] }}</td>
                            <td>{{ $food['bestBeforeDate'] }}</td>
                            <td>{{ $food['donationAmount'] }}{{ $food['donationUnit'] }}</td>
                            <td>
                                @if ($food['matchingState'] == 0)
                                未マッチ
                                @elseif ($food['matchingState'] == 1)
                                マッチ済み
                                @elseif ($food['matchingState'] == 2)
                                クローズ済み
                                @endif
                            </td>
                            <td class="text-nowrap">
                                <a href="/partners/food/edit/{{$food['foodId']}}" class="btn mx-1 btn-sm btn-outline-secondary JSLoderBtn">Edit</a>
                                <button type="button" data-name="{{ $food['foodName'] }}" data-id="{{$food['foodId']}}" class="openModal btn mx-1 btn-sm btn-outline-secondary">Delete</button>
                            </td>
                        </tr>
                    @endforeach
                    @endif
                    </tbody>
                </table>
            </div>
        </div>
        </div>
            @if(isset($foodsList['data']) && $foodsList['data'])
                {{$foodsList['data']->links('vendor.pagination.pagination')}}
            @endif
    </div>
</div>

<!-- モーダルエリアここから -->
<section id="modalArea" class="modalArea">
<div id="modalBg" class="modalBg"></div>
    <div class="modalWrapper">
    <div class="modalContents">
        <p><span id="foodName"></span>を削除しますか？</p>
        <form action="{{ route('foodDelete') }}" method="POST">
            @method('DELETE')
            @csrf
            <input type="text" name="foodId" id="getfoodid" hidden>
            <a id="cancel" class="btn btn-secondary m-1">キャンセル</a>
            <button class="btn btn-danger m-1">削除</button>
        </form>
    </div>
    <div id="closeModal" class="closeModal">×</div>
    </div>
</section>
<!-- モーダルエリアここまで -->


<script>
    $(function () {
    $('.openModal').click(function(){
        $('#modalArea').fadeIn();
        var foodId = $(this).data('id');
        var foodName = $(this).data('name');
        $('#foodName').text(foodName);
        $('#getfoodid').val(foodId);

    });
    $('#closeModal , #modalBg, #cancel').click(function(){
        $('#modalArea').fadeOut();
    });
});
</script>
@endsection
