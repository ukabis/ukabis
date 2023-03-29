<!doctype html>
<html lang="{{ str_replace('_', '-', app()->getLocale()) }}">
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">

    <!-- CSRF Token -->
    <meta name="csrf-token" content="{{ csrf_token() }}">

    <title>こども食堂食材提供システム</title>

    <!-- Fonts -->
    <link rel="dns-prefetch" href="//fonts.gstatic.com">
    {{-- <link href="https://fonts.bunny.net/css?family=Nunito" rel="stylesheet"> --}}
    <link rel="stylesheet" href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined:opsz,wght,FILL,GRAD@20..48,100..700,0..1,-50..200" />

    <!-- Scripts -->
    <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@4.6.0/dist/js/bootstrap.bundle.min.js"></script>
    @if(env('APP_ENV') == 'local')
        @vite(['resources/sass/app.scss', 'resources/js/app.js'])
    @else
        <link rel="stylesheet" href="{!! asset('build/assets/app-2211cb9e.css') !!}">
        <script src="{!! asset('build/assets/app-7edb886c.js') !!}"></script>
    @endif


    <!-- bootstrap-datepicker -->
    <link rel="stylesheet" type="text/css" href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.9.0/css/bootstrap-datepicker.min.css">
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.9.0/js/bootstrap-datepicker.min.js"></script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.9.0/locales/bootstrap-datepicker.ja.min.js"></script>
</head>
<body>
    <div id="loader">
        <div class="spinner">
            <div class="cube1"></div>
            <div class="cube2"></div>
        </div>
    </div>

    <div id="app">
        <nav class="navbar navbar-expand-md navbar-light bg-white shadow-sm">
            <div class="container">
                <a class="navbar-brand container JSLoderBtn col-md-6" href="{{ url('/') }}">
                    <img src="{{ asset('images/logo.png') }}" alt="ukabis"><div>こども食堂食材提供システム</div>
                </a>
                <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarSupportedContent" aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="{{ __('Toggle navigation') }}">
                    <span class="navbar-toggler-icon"></span>
                </button>

                <div class="collapse navbar-collapse" id="navbarSupportedContent">
                    <!-- Left Side Of Navbar -->
                    <ul class="navbar-nav me-auto">

                    </ul>

                    <!-- Right Side Of Navbar -->
                    <ul class="navbar-nav ms-auto">
                        <!-- Authentication Links -->

                        @if(session('__user'))
                            <li class="nav-item dropdown">
                                <a id="navbarDropdown" class="nav-link dropdown-toggle" href="#" role="button" data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false" v-pre>
                                    {{ session('__user')->familyName }} {{ session('__user')->givenName }}様
                                </a>

                                <div class="dropdown-menu dropdown-menu-end" aria-labelledby="navbarDropdown">
                                    <a class="dropdown-item" href="/partners/logout">ログアウト</a>
                                </div>
                            </li>

                        @else
                            <li class="nav-item">
                                <a class="nav-link" href="/partners/login">ログイン</a>
                            </li>

                            <li class="nav-item">
                                <a class="nav-link" href="/partners/register">新規登録</a>
                            </li>
                        @endif
                    </ul>
                </div>
            </div>
        </nav>
        <main class="py-4">
            @if(session('success'))
                <div class="container">
                    <div class="alert alert-success alert-dismissible fade show">
                        <div>{{ session('success') }}</div>
                        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
                    </div>
                </div>
            @endif
            @if(session('fail'))
                <div class="container">
                    <div class="alert alert-danger alert-dismissible fade show">
                        <div>{{ session('fail') }}</div>
                        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
                    </div>
                </div>
            @endif

            @if(isset($success) && $success)
                <div class="container">
                    <div class="alert alert-success alert-dismissible fade show">
                        <div>{{ $success }}</div>
                        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
                    </div>
                </div>
            @endif
            @if(isset($fail) && $fail)
                <div class="container">
                    <div class="alert alert-danger alert-dismissible fade show">
                        <div>{{ $fail }}</div>
                        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
                    </div>
                </div>
            @endif


            @yield('content')
        </main>
        <footer class="text-center text-white bg-ukabis-yewllow">
            <div class="text-center p-3 bg-ukabis-red">
                © kodomo.ukabis.jp
            </div>
            <!-- Copyright -->
        </footer>
    </div>
</body>
</html>
