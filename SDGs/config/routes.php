<?php
/**
 * Routes configuration.
 *
 * In this file, you set up routes to your controllers and their actions.
 * Routes are very important mechanism that allows you to freely connect
 * different URLs to chosen controllers and their actions (functions).
 *
 * It's loaded within the context of `Application::routes()` method which
 * receives a `RouteBuilder` instance `$routes` as method argument.
 *
 * CakePHP(tm) : Rapid Development Framework (https://cakephp.org)
 * Copyright (c) Cake Software Foundation, Inc. (https://cakefoundation.org)
 *
 * Licensed under The MIT License
 * For full copyright and license information, please see the LICENSE.txt
 * Redistributions of files must retain the above copyright notice.
 *
 * @copyright     Copyright (c) Cake Software Foundation, Inc. (https://cakefoundation.org)
 * @link          https://cakephp.org CakePHP(tm) Project
 * @license       https://opensource.org/licenses/mit-license.php MIT License
 */

use App\Middleware\CheckerFarmApplicationMiddleware;
use App\Middleware\CheckerRestaurantApplicationMiddleware;
use App\Middleware\CheckingRegisterPresidentMiddleware;
use Cake\Routing\Route\DashedRoute;
use Cake\Routing\RouteBuilder;

return static function (RouteBuilder $routes) {
    /*
     * The default class to use for all routes
     *
     * The following route classes are supplied with CakePHP and are appropriate
     * to set as the default:
     *
     * - Route
     * - InflectedRoute
     * - DashedRoute
     *
     * If no call is made to `Router::defaultRouteClass()`, the class used is
     * `Route` (`Cake\Routing\Route\Route`)
     *
     * Note that `Route` does not do any inflections on URLs which will result in
     * inconsistently cased URLs when used with `{plugin}`, `{controller}` and
     * `{action}` markers.
     */
    $routes->setRouteClass(DashedRoute::class);

    $routes->scope('/', function (RouteBuilder $builder) {
        /*
         * Here, we are connecting '/' (base path) to a controller called 'Pages',
         * its action called 'display', and we pass a param to select the view file
         * to use (in this case, templates/Pages/home.php)...
         */
        $builder->registerMiddleware('checkingRegisterPresident', new CheckingRegisterPresidentMiddleware());
        $builder->registerMiddleware('checkerFarmApplicationScored', new CheckerFarmApplicationMiddleware());
        $builder->registerMiddleware('checkerRestaurantApplicationScored', new CheckerRestaurantApplicationMiddleware());

        /*
         * Restaurants
         * */
        $builder->scope('/restaurants', ['_namePrefix' => 'restaurants:'] ,function (RouteBuilder $builder) {
            /*
             * Authentication
             * */
            $builder->connect('/login', 'Restaurants/Auth::login');
            $builder->connect('/logout', 'Restaurants/Auth::logout');

            /*
             * Register
             * */
            $builder->connect('/register', 'Restaurants/Register::register');
            $builder->connect('/president/register', 'Restaurants/President::register', ['_name' => 'presidentRegister']);
            $builder->connect('/president/edit', 'Restaurants/President::edit')->setMiddleware(['checkingRegisterPresident']);

            /*
             * My Page
             * */
            $builder->connect('/', 'Restaurants/Restaurant::index')->setMiddleware(['checkingRegisterPresident']);;
            $builder->connect('/edit', 'Restaurants/Restaurant::edit')->setMiddleware(['checkingRegisterPresident']);;
            $builder->connect('/detail', 'Restaurants/Restaurant::detail')->setMiddleware(['checkingRegisterPresident']);;
            $builder->connect('/inquiry', 'Restaurants/Restaurant::inquiry')->setMiddleware(['checkingRegisterPresident']);;

            /*
             * Application information
             * */
            $builder->scope('/application', function (RouteBuilder $builder) {
                $builder->connect('/question/{screen}', 'Restaurants/Application::index')
                    ->setPass(['screen'])
                    ->setMiddleware(['checkingRegisterPresident']);
                $builder->connect('/confirmation', 'Restaurants/Application::confirmation');
                $builder->connect('/complete', 'Restaurants/Application::complete');
            });
            $builder->connect('/upload-file', 'Restaurants/Application::upload')->setMiddleware(['checkingRegisterPresident']);

            $builder->scope('/answer', function (RouteBuilder $builder) {
                $builder->applyMiddleware('checkingRegisterPresident');
                $builder->connect('/apply', 'Restaurants/Application::store');
                $builder->connect('/show', 'Restaurants/Application::show');
            });

            /**
             * Request
             */
            $builder->scope('/request', function (RouteBuilder $builder) {
                $builder->applyMiddleware('checkingRegisterPresident');
                $builder->connect('/register', 'Restaurants/Request::register');
                $builder->connect('/list', 'Restaurants/Request::list');
            });

            /**
             * Farm request
             */
            $builder->scope('/farm-requests', function (RouteBuilder $builder) {
                $builder->applyMiddleware('checkingRegisterPresident');
                $builder->connect('/inquiry', 'Restaurants/FarmRequest::inquiry');
                $builder->connect('/search', 'Restaurants/FarmRequest::search');
                $builder->connect('/detail/{requestId}',
                    [
                        'prefix' => 'Restaurants',
                        'controller' => 'FarmRequest',
                        'action' => 'detail'
                    ],
                    ['requestId' => '\d+', 'pass' => ['requestId']]);
            });

            /**
             * Search farm
             */
            $builder->scope('/search', function (RouteBuilder $builder) {
                $builder->applyMiddleware('checkingRegisterPresident');
                $builder->connect('/', 'Restaurants/Search::index');
                $builder->connect('/farms', 'Restaurants/Search::list');
                $builder->connect('/office/{officeId}',
                    [
                        'prefix' => 'Restaurants',
                        'controller' => 'Search',
                        'action' => 'detail'
                    ],
                    [
                        'officeId' => '\d+', 'pass' => ['officeId']
                    ]
                );
            });

            /*
             * Download action
             * */
            $builder->scope('/download',  function (RouteBuilder $builder) {
                $builder->applyMiddleware('checkingRegisterPresident');
                $builder->connect('/', 'Restaurants/Restaurant::download');
                $builder->connect('/certificate', 'Restaurants/Download::downloadCertificate');
            });
        });

        /*
         * Farms
         * */
        $builder->scope('/farms', ['_namePrefix' => 'farms:'], function (RouteBuilder $builder) {
            /*
             * Authentication
             * */
            $builder->connect('/login', 'Farms/Auth::login');
            $builder->connect('/logout', 'Farms/Auth::logout');

            /*
             * Register
             * */
            $builder->connect('/register', 'Farms/Register::register');
            $builder->connect('/president/register', 'Farms/President::register', ['_name' => 'presidentRegister']);
            $builder->connect('/president/edit', 'Farms/President::edit')->setMiddleware(['checkingRegisterPresident']);

            /*
             * My Page
             * */
            $builder->connect('/', 'Farms/Farm::index')->setMiddleware(['checkingRegisterPresident']);
            $builder->connect('/edit', 'Farms/Farm::edit')->setMiddleware(['checkingRegisterPresident']);
            $builder->connect('/detail', 'Farms/Farm::detail')->setMiddleware(['checkingRegisterPresident']);

            /*
             * Application information
             * */
            $builder->scope('/application', function (RouteBuilder $builder) {
                $builder->connect('/question/{screen}', 'Farms/Application::index')
                    ->setPass(['screen'])
                    ->setMiddleware(['checkingRegisterPresident']);
                $builder->connect('/confirmation', 'Farms/Application::confirmation');
                $builder->connect('/complete', 'Farms/Application::complete');
            });
            $builder->connect('/upload-file', 'Farms/Application::upload')->setMiddleware(['checkingRegisterPresident']);

            $builder->scope('/answer', function (RouteBuilder $builder) {
                $builder->applyMiddleware('checkingRegisterPresident');
                $builder->connect('/apply', 'Farms/Application::store');
                $builder->connect('/show', 'Farms/Application::show');
            });

            /**
             * Request
             */
            $builder->scope('/request', function (RouteBuilder $builder) {
                $builder->connect('/register', 'Farms/Request::register')->setMiddleware(['checkingRegisterPresident']);
                $builder->connect('/list', 'Farms/Request::list')->setMiddleware(['checkingRegisterPresident']);
            });


            /**
             * Restaurant request
             */
            $builder->scope('/restaurant-requests', function (RouteBuilder $builder) {
                $builder->applyMiddleware('checkingRegisterPresident');
                $builder->connect('/inquiry', 'Farms/RestaurantRequest::inquiry');
                $builder->connect('/search', 'Farms/RestaurantRequest::search');
                $builder->connect('/detail/{requestId}',
                    [
                        'prefix' => 'Farms',
                        'controller' => 'RestaurantRequest',
                        'action' => 'detail'
                    ],
                    ['requestId' => '\d+', 'pass' => ['requestId']]
                );
            });

            /**
             * Search restaurant
             */
            $builder->scope('/search', function (RouteBuilder $builder) {
                $builder->applyMiddleware('checkingRegisterPresident');
                $builder->connect('/', 'Farms/Search::index');
                $builder->connect('/farms', 'Farms/Search::list');
                $builder->connect('/office/{officeId}',
                    [
                        'prefix' => 'Farms',
                        'controller' => 'Search',
                        'action' => 'detail'
                    ],
                    [
                        'officeId' => '\d+', 'pass' => ['officeId']
                    ]
                );
            });

            /*
             * Download action
             * */
            $builder->scope('/download',  function (RouteBuilder $builder) {
                $builder->applyMiddleware('checkingRegisterPresident');
                $builder->connect('/', 'Farms/Farm::download');
                $builder->connect('/certificate', 'Farms/Download::downloadCertificate');
            });
        });

        /*
         * Checker
         * */
        $builder->scope('/checker', ['_namePrefix' => 'checker:'], function (RouteBuilder $builder) {
            /*
             * Authentication
             * */
             $builder->connect('/login', 'Checker/Auth::login');
             $builder->connect('/logout', 'Checker/Auth::logout');

            /*
             * My Page
             * */
            $builder->connect('/', 'Checker/Checker::index');
            $builder->connect('/register', 'Checker/Checker::register');
            $builder->connect('/list', 'Checker/Checker::list');
            $builder->connect('/{id}',
                [
                    'prefix' => 'Checker',
                    'controller' => 'Checker',
                    'action' => 'detail'
                ],
                ['id' => '\d+', 'pass' => ['id']]);
            $builder->connect('/edit/{id}',
                [
                    'prefix' => 'Checker',
                    'controller' => 'Checker',
                    'action' => 'edit'
                ],
                ['id' => '\d+', 'pass' => ['id']]);
            $builder->connect('/delete/{id}',
                [
                    'prefix' => 'Checker',
                    'controller' => 'Checker',
                    'action' => 'delete'
                ],
                ['id' => '\d+', 'pass' => ['id']]);

            /*
             * Restaurant Application
             * */
            $builder->scope('/restaurants', function (RouteBuilder $builder) {
                $builder->connect('/list', 'Checker/RestaurantApplication::list');
                $builder->connect('/score/{id}',
                    [
                        'prefix' => 'Checker',
                        'controller' => 'RestaurantApplication',
                        'action' => 'score'
                    ],
                    ['id' => '\d+', 'pass' => ['id']]);
                $builder->connect('/total/{id}',
                    [
                        'prefix' => 'Checker',
                        'controller' => 'RestaurantApplication',
                        'action' => 'total'
                    ],
                    ['id' => '\d+', 'pass' => ['id']]
                )->setMiddleware(['checkerRestaurantApplicationScored']);
                $builder->connect('/email/{id}',
                    [
                        'prefix' => 'Checker',
                        'controller' => 'RestaurantApplication',
                        'action' => 'sendMail'
                    ],
                    ['id' => '\d+', 'pass' => ['id']]
                )->setMiddleware(['checkerRestaurantApplicationScored']);;
            });

            /*
             * Farm Application
             * */
            $builder->scope('/farms', function (RouteBuilder $builder) {
                $builder->connect('/list', 'Checker/FarmApplication::list');
                $builder->connect('/score/{id}',
                    [
                        'prefix' => 'Checker',
                        'controller' => 'FarmApplication',
                        'action' => 'score'
                    ],
                    ['id' => '\d+', 'pass' => ['id']]
                );
                $builder->connect('/score-total/{id}',
                    [
                        'prefix' => 'Checker',
                        'controller' => 'FarmApplication',
                        'action' => 'scoreTotal'
                    ],
                    ['id' => '\d+', 'pass' => ['id']]
                )->setMiddleware(['checkerFarmApplicationScored']);
                $builder->connect('/email/{id}',
                    [
                        'prefix' => 'Checker',
                        'controller' => 'FarmApplication',
                        'action' => 'sendMail'
                    ],
                    ['id' => '\d+', 'pass' => ['id']]
                )->setMiddleware(['checkerFarmApplicationScored']);
            });
        });

        /*
         * Admin auth and dashboard
         * */
        $builder->scope('/admin', ['prefix' => 'Admin'], function (RouteBuilder $builder) {
            /*
             * Authentication
             * */
             $builder->connect('/login', 'Auth::login', ['_name' => 'admin.login']);
             $builder->connect('/logout', 'Auth::logout', ['_name' => 'admin.logout']);

            /*
             * My Page
             * */
            $builder->connect('/', 'Admin::index', ['_name' => 'admin.index']);
            $builder->connect('/register', 'Admin::register', ['_name' => 'admin.register']);
            $builder->connect('/list', 'Admin::list', ['_name' => 'admin.list']);
            $builder->connect('/edit/{id}',
                        [
                            'controller' => 'Admin',
                            'action' => 'edit'
                        ],
                        ['id' => '\d+', 'pass' => ['id'], '_name' => 'admin.edit']);
            $builder->connect('/detail/{id}',
                        [
                            'controller' => 'Admin',
                            'action' => 'detail'
                        ],
                        ['id' => '\d+', 'pass' => ['id'], '_name' => 'admin.detail']);
            $builder->connect('/delete/{id}',
                        [
                            'controller' => 'Admin',
                            'action' => 'delete'
                        ],
                        ['id' => '\d+', 'pass' => ['id'], '_name' => 'admin.delete']);

            $builder->scope('/checker', function (RouteBuilder $builder) {
                $builder->connect('/', 'Checker::index', ['_name' => 'admin.checker.index']);
                $builder->connect('/register', 'Checker::register', ['_name' => 'admin.checker.register']);
                $builder->connect('/list', 'Checker::list', ['_name' => 'admin.checker.list']);
                $builder->connect('/edit/{id}',
                            [
                                'controller' => 'Checker',
                                'action' => 'edit'
                            ],
                            ['id' => '\d+', 'pass' => ['id'], '_name' => 'admin.checker.edit']);
                $builder->connect('/detail/{id}',
                            [
                                'controller' => 'Checker',
                                'action' => 'detail'
                            ],
                            ['id' => '\d+', 'pass' => ['id'], '_name' => 'admin.checker.detail']);
                $builder->connect('/delete/{id}',
                            [
                                'controller' => 'Checker',
                                'action' => 'delete'
                            ],
                            ['id' => '\d+', 'pass' => ['id'], '_name' => 'admin.checker.delete']);
            });
        });

        /*
         * User
         * */
        $builder->connect('/', ['controller' => 'Home', 'action' => 'index']);
        $builder->scope('/search', function (RouteBuilder $builder) {
            $builder->connect('/restaurant', ['controller' => 'SearchRestaurant', 'action' => 'list']);
            $builder->connect('/restaurant/{id}',
                [
                    'controller' => 'SearchRestaurant',
                    'action' => 'detail'
                ],
                ['id' => '\d+', 'pass' => ['id']]
            );
            $builder->connect('/farm', ['controller' => 'SearchFarm', 'action' => 'list']);
            $builder->connect('/farm/{id}',
                [
                    'controller' => 'SearchFarm',
                    'action' => 'detail'
                ],
                ['id' => '\d+', 'pass' => ['id']]
            );
        });

        /*
         * ...and connect the rest of 'Pages' controller's URLs.
         */
        $builder->connect('/pages/*', 'Pages::display');

        /*
         * Connect catchall routes for all controllers.
         *
         * The `fallbacks` method is a shortcut for
         *
         * ```
         * $builder->connect('/{controller}', ['action' => 'index']);
         * $builder->connect('/{controller}/{action}/*', []);
         * ```
         *
         * You can remove these routes once you've connected the
         * routes you want in your application.
         */
        $builder->fallbacks();
    });

    /*
     * If you need a different set of middleware or none at all,
     * open new scope and define routes there.
     *
     * ```
     * $routes->scope('/api', function (RouteBuilder $builder) {
     *     // No $builder->applyMiddleware() here.
     *
     *     // Parse specified extensions from URLs
     *     // $builder->setExtensions(['json', 'xml']);
     *
     *     // Connect API actions here.
     * });
     * ```
     */
};
