@extends('layouts.app')

@section('content')
<div class="container">
    <div class="row justify-content-center">
        <div class="col-md-8">
            <div class="card">
                <div class="card-header">{{ __('Dashboard') }}</div>

                <div class="card-body">
                    @if (session('status'))
                        <div class="alert alert-success" role="alert">
                            {{ session('status') }}
                        </div>
                    @endif

                        <table>
                            <thead>
                            <tr>
                                <th width="30%">Product Name</th>
                                <th width="30%">Description</th>
                                <th width="30%">Price</th>
                                <th width="30%"></th>
                            </tr>
                            </thead>
                            <tbody>
                            <tr>
                                <td>Product 1</td>
                                <td>Description of Product 1</td>
                                <td>$10.99</td>
                                <td><button>Add to Cart</button></td>
                            </tr>
                            <tr>
                                <td>Product 2</td>
                                <td>Description of Product 2</td>
                                <td>$19.99</td>
                                <td><button>Add to Cart</button></td>
                            </tr>
                            <tr>
                                <td>Product 3</td>
                                <td>Description of Product 3</td>
                                <td>$5.99</td>
                                <td><button>Add to Cart</button></td>
                            </tr>
                            </tbody>
                        </table>



                        {{ __('You are logged in!') }}
                </div>
            </div>
        </div>
    </div>
</div>
@endsection
