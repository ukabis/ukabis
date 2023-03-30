<?php

use Illuminate\Support\Facades\Auth;
use Illuminate\Support\Facades\Route;

/*
|--------------------------------------------------------------------------
| Web Routes
|--------------------------------------------------------------------------
|
| Here is where you can register web routes for your application. These
| routes are loaded by the RouteServiceProvider within a group which
| contains the "web" middleware group. Now create something great!
|
*/


Route::get('/', function () {
    return redirect('/partners');
});

Route::group(['prefix' => 'partners'], function () {
    Route::get('/', 'App\Http\Controllers\FoodController@foodsList');

    Route::get('/register', 'App\Http\Controllers\RegisterController@index')->name('register.index');
    Route::post('/register', 'App\Http\Controllers\RegisterController@index');
    Route::get('/register/code', 'App\Http\Controllers\RegisterController@code')->name('register.code');
    Route::post('/register/code', 'App\Http\Controllers\RegisterController@code');

    Route::get('/terms', 'App\Http\Controllers\TermsController@terms');
    Route::post('/terms', 'App\Http\Controllers\TermsController@termsAgreement')->name('termsAgreement');

    Route::get('/login', 'App\Http\Controllers\AuthController@login');
    Route::post('/login', 'App\Http\Controllers\AuthController@login');

    Route::get('/change_pass', 'App\Http\Controllers\AuthController@change_pass');
    Route::post('/change_pass', 'App\Http\Controllers\AuthController@change_pass');

    Route::get('/logout', 'App\Http\Controllers\AuthController@logout');

    Route::get('/food', 'App\Http\Controllers\FoodController@foodsList')->name('foodsList');
    Route::get('/food/create', 'App\Http\Controllers\FoodController@create');
    Route::post('/food/create', 'App\Http\Controllers\FoodController@create')->name('foodCreate');
    Route::get('/food/edit/{foodId}', 'App\Http\Controllers\FoodController@edit')->name('food.edit');
    Route::delete('/food', 'App\Http\Controllers\FoodController@delete')->name('foodDelete');
});