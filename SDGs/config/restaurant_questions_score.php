<?php

use Cake\Core\Configure;

$restaurantQuestions = [
    '1' => [
        'type'                  => TYPE_MULTI_MENU,
        'title'                 => '静岡県内で生産された食材を使用したメニューを入力してください。',
        'sub_title_1'           => '※寿司・天ぷら・串カツ・ケーキなどの単品アイテムについては1品あたり静岡県産食材1品でも可。',
        'sub_title_2'           => '※地元食材とは、原則、県内隣接市町までの範囲内、自家菜園を含みます。',
        'answer_title'          => 'メニューを登録',
        'menu_table_image'      => '',
        'store_image'           => '',
        'menu_max_item'         => 10,
        'ingredient_max_item'   => 2,
        'menus' => [
            1 => [
                'name'                => '',
                'image'               => '',
                'public_period_start' => '',
                'public_period_end'   => '',
                'ingredients'   => [
                    1 => [
                        'name'              => '',
                        'supplier'          => '',
                        'local_ingredient'  => 0,
                        'extra_information' => [
                            'certification_ingredients' => [
                                'title' => '食材のSDGs認証を選択する。',
                                'name'  => 'certification_ingredients',
                                'type'  => 'radio',
                                'options' => [
                                    1 => [
                                        'label' => 'ゴールド認証',
                                        'score' => 3
                                    ],
                                    2 => [
                                        'label' => 'シルバー認証',
                                        'score' => 2
                                    ],
                                    3 => [
                                        'label' => 'ブロンズ認証',
                                        'score' => 1
                                    ],
                                    4 => [
                                        'label' => '認証なし',
                                        'score' => 0
                                    ],
                                ]
                            ],
                            'region_ingredients' => [
                                'title' => '本県または地元（地域）ならではの食材かを選択する。',
                                'name'  => 'region_ingredients',
                                'type'  => 'checkbox',
                                'options' => [
                                    1 => [
                                        'label' => 'しずおか食セレクション食材',
                                        'score' => 3
                                    ],
                                    2 => [
                                        'label' => '地域ブランド農林水産物の食材',
                                        'score' => 2
                                    ],
                                    3 => [
                                        'label' => 'その他（申告制：県内の在来種・希少種など）',
                                        'score' => 1
                                    ],
                                ]
                            ],
                            'ethical_ingredients' => [
                                'title' => 'エシカルな食材かを選択する。',
                                'name'  => 'ethical_ingredients',
                                'type'  => 'checkbox',
                                'options' => [
                                    1 => [
                                        'label' => 'オーガニック・有機ＪＡＳ',
                                        'score' => 6
                                    ],
                                    2 => [
                                        'label' => 'ＭＳＣ認証',
                                        'score' => 2
                                    ],
                                    3 => [
                                        'label' => 'その他（申告制：フェアトレード、アニマルウェルフェア、代替肉の利用など）',
                                        'score' => 1
                                    ],
                                ]
                            ],
                        ]
                    ]
                ]
            ],
            2 => [
                'name'          => '',
                'image'         => '',
                'public_period' => '',
                'ingredients'   => [
                    1 => [
                        'name'              => '',
                        'supplier'          => '',
                        'local_ingredient'  => 0,
                        'extra_information' => [
                            'certification_ingredients' => [
                                'title' => '食材のSDGs認証を選択する。',
                                'name'  => 'certification_ingredients',
                                'type'  => 'radio',
                                'options' => [
                                    1 => [
                                        'label' => 'ゴールド認証',
                                        'score' => 3
                                    ],
                                    2 => [
                                        'label' => 'シルバー認証',
                                        'score' => 2
                                    ],
                                    3 => [
                                        'label' => 'ブロンズ認証',
                                        'score' => 1
                                    ],
                                    4 => [
                                        'label' => '認証なし',
                                        'score' => 0
                                    ],
                                ]
                            ],
                            'region_ingredients' => [
                                'title' => '本県または地元（地域）ならではの食材かを選択する。',
                                'name'  => 'region_ingredients',
                                'type'  => 'checkbox',
                                'options' => [
                                    1 => [
                                        'label' => 'しずおか食セレクション食材',
                                        'score' => 3
                                    ],
                                    2 => [
                                        'label' => '地域ブランド農林水産物の食材',
                                        'score' => 2
                                    ],
                                    3 => [
                                        'label' => 'その他（申告制：県内の在来種・希少種など）',
                                        'score' => 1
                                    ],
                                ]
                            ],
                            'ethical_ingredients' => [
                                'title' => 'エシカルな食材かを選択する。',
                                'name'  => 'ethical_ingredients',
                                'type'  => 'checkbox',
                                'options' => [
                                    1 => [
                                        'label' => 'オーガニック・有機ＪＡＳ',
                                        'score' => 6
                                    ],
                                    2 => [
                                        'label' => 'ＭＳＣ認証',
                                        'score' => 2
                                    ],
                                    3 => [
                                        'label' => 'その他（申告制：フェアトレード、アニマルウェルフェア、代替肉の利用など）',
                                        'score' => 1
                                    ],
                                ]
                            ],
                        ]
                    ]
                ]
            ],
            3 => [
                'name'          => '',
                'image'         => '',
                'public_period' => '',
                'ingredients'   => [
                    1 => [
                        'name'              => '',
                        'supplier'          => '',
                        'local_ingredient'  => 0,
                        'extra_information' => [
                            'certification_ingredients' => [
                                'title' => '食材のSDGs認証を選択する。',
                                'name'  => 'certification_ingredients',
                                'type'  => 'radio',
                                'options' => [
                                    1 => [
                                        'label' => 'ゴールド認証',
                                        'score' => 3
                                    ],
                                    2 => [
                                        'label' => 'シルバー認証',
                                        'score' => 2
                                    ],
                                    3 => [
                                        'label' => 'ブロンズ認証',
                                        'score' => 1
                                    ],
                                    4 => [
                                        'label' => '認証なし',
                                        'score' => 0
                                    ],
                                ]
                            ],
                            'region_ingredients' => [
                                'title' => '本県または地元（地域）ならではの食材かを選択する。',
                                'name'  => 'region_ingredients',
                                'type'  => 'checkbox',
                                'options' => [
                                    1 => [
                                        'label' => 'しずおか食セレクション食材',
                                        'score' => 3
                                    ],
                                    2 => [
                                        'label' => '地域ブランド農林水産物の食材',
                                        'score' => 2
                                    ],
                                    3 => [
                                        'label' => 'その他（申告制：県内の在来種・希少種など）',
                                        'score' => 1
                                    ],
                                ]
                            ],
                            'ethical_ingredients' => [
                                'title' => 'エシカルな食材かを選択する。',
                                'name'  => 'ethical_ingredients',
                                'type'  => 'checkbox',
                                'options' => [
                                    1 => [
                                        'label' => 'オーガニック・有機ＪＡＳ',
                                        'score' => 6
                                    ],
                                    2 => [
                                        'label' => 'ＭＳＣ認証',
                                        'score' => 2
                                    ],
                                    3 => [
                                        'label' => 'その他（申告制：フェアトレード、アニマルウェルフェア、代替肉の利用など）',
                                        'score' => 1
                                    ],
                                ]
                            ],
                        ]
                    ]
                ]
            ],
            4 => [
                'name'          => '',
                'image'         => '',
                'public_period' => '',
                'ingredients'   => [
                    1 => [
                        'name'              => '',
                        'supplier'          => '',
                        'local_ingredient'  => 0,
                        'extra_information' => [
                            'certification_ingredients' => [
                                'title' => '食材のSDGs認証を選択する。',
                                'name'  => 'certification_ingredients',
                                'type'  => 'radio',
                                'options' => [
                                    1 => [
                                        'label' => 'ゴールド認証',
                                        'score' => 3
                                    ],
                                    2 => [
                                        'label' => 'シルバー認証',
                                        'score' => 2
                                    ],
                                    3 => [
                                        'label' => 'ブロンズ認証',
                                        'score' => 1
                                    ],
                                    4 => [
                                        'label' => '認証なし',
                                        'score' => 0
                                    ],
                                ]
                            ],
                            'region_ingredients' => [
                                'title' => '本県または地元（地域）ならではの食材かを選択する。',
                                'name'  => 'region_ingredients',
                                'type'  => 'checkbox',
                                'options' => [
                                    1 => [
                                        'label' => 'しずおか食セレクション食材',
                                        'score' => 3
                                    ],
                                    2 => [
                                        'label' => '地域ブランド農林水産物の食材',
                                        'score' => 2
                                    ],
                                    3 => [
                                        'label' => 'その他（申告制：県内の在来種・希少種など）',
                                        'score' => 1
                                    ],
                                ]
                            ],
                            'ethical_ingredients' => [
                                'title' => 'エシカルな食材かを選択する。',
                                'name'  => 'ethical_ingredients',
                                'type'  => 'checkbox',
                                'options' => [
                                    1 => [
                                        'label' => 'オーガニック・有機ＪＡＳ',
                                        'score' => 6
                                    ],
                                    2 => [
                                        'label' => 'ＭＳＣ認証',
                                        'score' => 2
                                    ],
                                    3 => [
                                        'label' => 'その他（申告制：フェアトレード、アニマルウェルフェア、代替肉の利用など）',
                                        'score' => 1
                                    ],
                                ]
                            ],
                        ]
                    ]
                ]
            ],
            5 => [
                'name'          => '',
                'image'         => '',
                'public_period' => '',
                'ingredients'   => [
                    1 => [
                        'name'              => '',
                        'supplier'          => '',
                        'local_ingredient'  => 0,
                        'extra_information' => [
                            'certification_ingredients' => [
                                'title' => '食材のSDGs認証を選択する。',
                                'name'  => 'certification_ingredients',
                                'type'  => 'radio',
                                'options' => [
                                    1 => [
                                        'label' => 'ゴールド認証',
                                        'score' => 3
                                    ],
                                    2 => [
                                        'label' => 'シルバー認証',
                                        'score' => 2
                                    ],
                                    3 => [
                                        'label' => 'ブロンズ認証',
                                        'score' => 1
                                    ],
                                    4 => [
                                        'label' => '認証なし',
                                        'score' => 0
                                    ],
                                ]
                            ],
                            'region_ingredients' => [
                                'title' => '本県または地元（地域）ならではの食材かを選択する。',
                                'name'  => 'region_ingredients',
                                'type'  => 'checkbox',
                                'options' => [
                                    1 => [
                                        'label' => 'しずおか食セレクション食材',
                                        'score' => 3
                                    ],
                                    2 => [
                                        'label' => '地域ブランド農林水産物の食材',
                                        'score' => 2
                                    ],
                                    3 => [
                                        'label' => 'その他（申告制：県内の在来種・希少種など）',
                                        'score' => 1
                                    ],
                                ]
                            ],
                            'ethical_ingredients' => [
                                'title' => 'エシカルな食材かを選択する。',
                                'name'  => 'ethical_ingredients',
                                'type'  => 'checkbox',
                                'options' => [
                                    1 => [
                                        'label' => 'オーガニック・有機ＪＡＳ',
                                        'score' => 6
                                    ],
                                    2 => [
                                        'label' => 'ＭＳＣ認証',
                                        'score' => 2
                                    ],
                                    3 => [
                                        'label' => 'その他（申告制：フェアトレード、アニマルウェルフェア、代替肉の利用など）',
                                        'score' => 1
                                    ],
                                ]
                            ],
                        ]
                    ]
                ]
            ],
            6 => [
                'name'          => '',
                'image'         => '',
                'public_period' => '',
                'ingredients'   => [
                    1 => [
                        'name'              => '',
                        'supplier'          => '',
                        'local_ingredient'  => 0,
                        'extra_information' => [
                            'certification_ingredients' => [
                                'title' => '食材のSDGs認証を選択する。',
                                'name'  => 'certification_ingredients',
                                'type'  => 'radio',
                                'options' => [
                                    1 => [
                                        'label' => 'ゴールド認証',
                                        'score' => 3
                                    ],
                                    2 => [
                                        'label' => 'シルバー認証',
                                        'score' => 2
                                    ],
                                    3 => [
                                        'label' => 'ブロンズ認証',
                                        'score' => 1
                                    ],
                                    4 => [
                                        'label' => '認証なし',
                                        'score' => 0
                                    ],
                                ]
                            ],
                            'region_ingredients' => [
                                'title' => '本県または地元（地域）ならではの食材かを選択する。',
                                'name'  => 'region_ingredients',
                                'type'  => 'checkbox',
                                'options' => [
                                    1 => [
                                        'label' => 'しずおか食セレクション食材',
                                        'score' => 3
                                    ],
                                    2 => [
                                        'label' => '地域ブランド農林水産物の食材',
                                        'score' => 2
                                    ],
                                    3 => [
                                        'label' => 'その他（申告制：県内の在来種・希少種など）',
                                        'score' => 1
                                    ],
                                ]
                            ],
                            'ethical_ingredients' => [
                                'title' => 'エシカルな食材かを選択する。',
                                'name'  => 'ethical_ingredients',
                                'type'  => 'checkbox',
                                'options' => [
                                    1 => [
                                        'label' => 'オーガニック・有機ＪＡＳ',
                                        'score' => 6
                                    ],
                                    2 => [
                                        'label' => 'ＭＳＣ認証',
                                        'score' => 2
                                    ],
                                    3 => [
                                        'label' => 'その他（申告制：フェアトレード、アニマルウェルフェア、代替肉の利用など）',
                                        'score' => 1
                                    ],
                                ]
                            ],
                        ]
                    ]
                ]
            ],
            7 => [
                'name'          => '',
                'image'         => '',
                'public_period' => '',
                'ingredients'   => [
                    1 => [
                        'name'              => '',
                        'supplier'          => '',
                        'local_ingredient'  => 0,
                        'extra_information' => [
                            'certification_ingredients' => [
                                'title' => '食材のSDGs認証を選択する。',
                                'name'  => 'certification_ingredients',
                                'type'  => 'radio',
                                'options' => [
                                    1 => [
                                        'label' => 'ゴールド認証',
                                        'score' => 3
                                    ],
                                    2 => [
                                        'label' => 'シルバー認証',
                                        'score' => 2
                                    ],
                                    3 => [
                                        'label' => 'ブロンズ認証',
                                        'score' => 1
                                    ],
                                    4 => [
                                        'label' => '認証なし',
                                        'score' => 0
                                    ],
                                ]
                            ],
                            'region_ingredients' => [
                                'title' => '本県または地元（地域）ならではの食材かを選択する。',
                                'name'  => 'region_ingredients',
                                'type'  => 'checkbox',
                                'options' => [
                                    1 => [
                                        'label' => 'しずおか食セレクション食材',
                                        'score' => 3
                                    ],
                                    2 => [
                                        'label' => '地域ブランド農林水産物の食材',
                                        'score' => 2
                                    ],
                                    3 => [
                                        'label' => 'その他（申告制：県内の在来種・希少種など）',
                                        'score' => 1
                                    ],
                                ]
                            ],
                            'ethical_ingredients' => [
                                'title' => 'エシカルな食材かを選択する。',
                                'name'  => 'ethical_ingredients',
                                'type'  => 'checkbox',
                                'options' => [
                                    1 => [
                                        'label' => 'オーガニック・有機ＪＡＳ',
                                        'score' => 6
                                    ],
                                    2 => [
                                        'label' => 'ＭＳＣ認証',
                                        'score' => 2
                                    ],
                                    3 => [
                                        'label' => 'その他（申告制：フェアトレード、アニマルウェルフェア、代替肉の利用など）',
                                        'score' => 1
                                    ],
                                ]
                            ],
                        ]
                    ]
                ]
            ],
            8 => [
                'name'          => '',
                'image'         => '',
                'public_period' => '',
                'ingredients'   => [
                    1 => [
                        'name'              => '',
                        'supplier'          => '',
                        'local_ingredient'  => 0,
                        'extra_information' => [
                            'certification_ingredients' => [
                                'title' => '食材のSDGs認証を選択する。',
                                'name'  => 'certification_ingredients',
                                'type'  => 'radio',
                                'options' => [
                                    1 => [
                                        'label' => 'ゴールド認証',
                                        'score' => 3
                                    ],
                                    2 => [
                                        'label' => 'シルバー認証',
                                        'score' => 2
                                    ],
                                    3 => [
                                        'label' => 'ブロンズ認証',
                                        'score' => 1
                                    ],
                                    4 => [
                                        'label' => '認証なし',
                                        'score' => 0
                                    ],
                                ]
                            ],
                            'region_ingredients' => [
                                'title' => '本県または地元（地域）ならではの食材かを選択する。',
                                'name'  => 'region_ingredients',
                                'type'  => 'checkbox',
                                'options' => [
                                    1 => [
                                        'label' => 'しずおか食セレクション食材',
                                        'score' => 3
                                    ],
                                    2 => [
                                        'label' => '地域ブランド農林水産物の食材',
                                        'score' => 2
                                    ],
                                    3 => [
                                        'label' => 'その他（申告制：県内の在来種・希少種など）',
                                        'score' => 1
                                    ],
                                ]
                            ],
                            'ethical_ingredients' => [
                                'title' => 'エシカルな食材かを選択する。',
                                'name'  => 'ethical_ingredients',
                                'type'  => 'checkbox',
                                'options' => [
                                    1 => [
                                        'label' => 'オーガニック・有機ＪＡＳ',
                                        'score' => 6
                                    ],
                                    2 => [
                                        'label' => 'ＭＳＣ認証',
                                        'score' => 2
                                    ],
                                    3 => [
                                        'label' => 'その他（申告制：フェアトレード、アニマルウェルフェア、代替肉の利用など）',
                                        'score' => 1
                                    ],
                                ]
                            ],
                        ]
                    ]
                ]
            ],
            9 => [
                'name'          => '',
                'image'         => '',
                'public_period' => '',
                'ingredients'   => [
                    1 => [
                        'name'              => '',
                        'supplier'          => '',
                        'local_ingredient'  => 0,
                        'extra_information' => [
                            'certification_ingredients' => [
                                'title' => '食材のSDGs認証を選択する。',
                                'name'  => 'certification_ingredients',
                                'type'  => 'radio',
                                'options' => [
                                    1 => [
                                        'label' => 'ゴールド認証',
                                        'score' => 3
                                    ],
                                    2 => [
                                        'label' => 'シルバー認証',
                                        'score' => 2
                                    ],
                                    3 => [
                                        'label' => 'ブロンズ認証',
                                        'score' => 1
                                    ],
                                    4 => [
                                        'label' => '認証なし',
                                        'score' => 0
                                    ],
                                ]
                            ],
                            'region_ingredients' => [
                                'title' => '本県または地元（地域）ならではの食材かを選択する。',
                                'name'  => 'region_ingredients',
                                'type'  => 'checkbox',
                                'options' => [
                                    1 => [
                                        'label' => 'しずおか食セレクション食材',
                                        'score' => 3
                                    ],
                                    2 => [
                                        'label' => '地域ブランド農林水産物の食材',
                                        'score' => 2
                                    ],
                                    3 => [
                                        'label' => 'その他（申告制：県内の在来種・希少種など）',
                                        'score' => 1
                                    ],
                                ]
                            ],
                            'ethical_ingredients' => [
                                'title' => 'エシカルな食材かを選択する。',
                                'name'  => 'ethical_ingredients',
                                'type'  => 'checkbox',
                                'options' => [
                                    1 => [
                                        'label' => 'オーガニック・有機ＪＡＳ',
                                        'score' => 6
                                    ],
                                    2 => [
                                        'label' => 'ＭＳＣ認証',
                                        'score' => 2
                                    ],
                                    3 => [
                                        'label' => 'その他（申告制：フェアトレード、アニマルウェルフェア、代替肉の利用など）',
                                        'score' => 1
                                    ],
                                ]
                            ],
                        ]
                    ]
                ]
            ],
            10 => [
                'name'          => '',
                'image'         => '',
                'public_period' => '',
                'ingredients'   => [
                    1 => [
                        'name'              => '',
                        'supplier'          => '',
                        'local_ingredient'  => 0,
                        'extra_information' => [
                            'certification_ingredients' => [
                                'title' => '食材のSDGs認証を選択する。',
                                'name'  => 'certification_ingredients',
                                'type'  => 'radio',
                                'options' => [
                                    1 => [
                                        'label' => 'ゴールド認証',
                                        'score' => 3
                                    ],
                                    2 => [
                                        'label' => 'シルバー認証',
                                        'score' => 2
                                    ],
                                    3 => [
                                        'label' => 'ブロンズ認証',
                                        'score' => 1
                                    ],
                                    4 => [
                                        'label' => '認証なし',
                                        'score' => 0
                                    ],
                                ]
                            ],
                            'region_ingredients' => [
                                'title' => '本県または地元（地域）ならではの食材かを選択する。',
                                'name'  => 'region_ingredients',
                                'type'  => 'checkbox',
                                'options' => [
                                    1 => [
                                        'label' => 'しずおか食セレクション食材',
                                        'score' => 3
                                    ],
                                    2 => [
                                        'label' => '地域ブランド農林水産物の食材',
                                        'score' => 2
                                    ],
                                    3 => [
                                        'label' => 'その他（申告制：県内の在来種・希少種など）',
                                        'score' => 1
                                    ],
                                ]
                            ],
                            'ethical_ingredients' => [
                                'title' => 'エシカルな食材かを選択する。',
                                'name'  => 'ethical_ingredients',
                                'type'  => 'checkbox',
                                'options' => [
                                    1 => [
                                        'label' => 'オーガニック・有機ＪＡＳ',
                                        'score' => 6
                                    ],
                                    2 => [
                                        'label' => 'ＭＳＣ認証',
                                        'score' => 2
                                    ],
                                    3 => [
                                        'label' => 'その他（申告制：フェアトレード、アニマルウェルフェア、代替肉の利用など）',
                                        'score' => 1
                                    ],
                                ]
                            ],
                        ]
                    ]
                ]
            ],
        ]
    ],
    '2' => [
        'type'              => TYPE_CHECKBOX,
        'title'             => 'フードロス削減に向けた取組について、以下より該当する取り組みを選択してください。',
        'menu_table_image'  => '',
        'effort_image'      => '',
        'options'   => [
            1 => [
                'label' => '未利用食材（規格外食材）の利用',
                'score' => 2
            ],
            2 => [
                'label' => 'ハーフサイズ・ミニサイズなど、お客様の要望に応じた分量でのメニュー提供',
                'score' => 2
            ],
            3 => [
                'label' => 'ＰＯＳレジなどを活用した現状把握に基づく無駄のない食材調達',
                'score' => 1
            ],
            4 => [
                'label' => '自店舗における独自の賞味期限などの設定（食品衛生管理シールの導入など）',
                'score' => 1
            ],
            5 => [
                'label' => 'その他（申告制：フードシェアリングへの参画など）',
                'score' => 1
            ],
        ]
    ],
    '3' => [
        'type'          => TYPE_CHECKBOX,
        'title'         => '省エネ・節電に向けた取組について、以下より該当する項目を選択してください。',
        'effort_image'  => '',
        'options'  => [
            1 => [
                'label' => 'こまめな消灯、照明の間引きなど、節電に向けた取組の実践',
                'score' => 2
            ],
            2 => [
                'label' => '自動点灯スイッチの導入、ＬＥＤ照明への交換など、機器導入による節電の実践',
                'score' => 2
            ],
            3 => [
                'label' => '店舗の冷やしすぎ、暖めすぎに注意した冷暖房の使用（室温の目安　夏季28℃、冬季20℃）',
                'score' => 1
            ],
            4 => [
                'label' => '冷暖房の効率化を図るための店舗内の工夫'. '<br>'.'（サーキュレーター・ブラインド、カーテン、遮断フィルム、ひさし、すだれ等の活用）',
                'score' => 1
            ],
            5 => [
                'label' => '集中スイッチなどを用いた、閉店・未使用時の待機電力の節減',
                'score' => 1
            ],
            6 => [
                'label' => '冷蔵庫内の在庫管理、適切な温度管理の実践',
                'score' => 1
            ],
            7 => [
                'label' => '環境配慮型厨房機器の導入（インバーター仕様・省エネ・エコ仕様など）',
                'score' => 1
            ],
            8 => [
                'label' => 'ゴミの分別',
                'score' => 1
            ],
            9 => [
                'label' => 'ゴミ袋、ポリ袋の使用の軽減への取組',
                'score' => 1
            ],
            10 => [
                'label' => 'バイオマス由来の持ち帰り容器の導入（木製・紙製・パルプモールド等）',
                'score' => 1
            ],
            11 => [
                'label' => '生ゴミの堆肥化（コンポスト利用等）の実践',
                'score' => 1
            ]
        ]
    ],
    '4' => [
        'type'      => TYPE_CHECKBOX,
        'title'     => 'グローバル対応について、以下より該当する取り組みについて選択してください。',
        'menu_table_image'  => '',
        'effort_image'      => '',
        'options'   => [
            1 => [
                'label' => '英語に対応したメニューなどの提示',
                'score' => 2
            ],
            2 => [
                'label' => '写真やイラスト図形を活用したわかりやすいメニューなどの提示',
                'score' => 1
            ],
            3 => [
                'label' => '指差しツールの活用など、コミュニケーションの工夫の実施',
                'score' => 1
            ],
            4 => [
                'label' => 'ピクトグラム等を活用した店内表示や衛生案内',
                'score' => 1
            ],
            5 => [
                'label' => '顧客向けWi-Fi環境の整備',
                'score' => 1
            ],
            6 => [
                'label' => 'キャッシュレス決済、カード決済の導入',
                'score' => 1
            ],
            7 => [
                'label' => 'WEBページ・インスタ等による情報発信の実施',
                'score' => 1
            ],
            8 => [
                'label' => 'その他（申告制）',
                'score' => 1
            ]
        ]
    ],
    '5' => [
        'type'      => TYPE_CHECKBOX,
        'title'     => 'バリアフリー対応について、以下より該当する取り組みについて選択してください。',
        'menu_table_image'  => '',
        'effort_image'      => '',
        'options'   => [
            1 => [
                'label' => '車椅子利用者に配慮した通路（店舗内の有効幅90㎝以上、床に段差なし）',
                'score' => 1
            ],
            2 => [
                'label' => 'テーブルや椅子などの配置変更',
                'score' => 1
            ],
            3 => [
                'label' => '車椅子利用者用トイレの設置',
                'score' => 1
            ],
            4 => [
                'label' => 'その他（申告制）',
                'score' => 1
            ]
        ]
    ],
    '6' => [
        'type'      => TYPE_CHECKBOX,
        'title'     => '多様な人々が利用できるための配慮について、以下より該当する取り組みについて選択してください。',
        'menu_table_image'  => '',
        'effort_image'      => '',
        'options'   => [
            1 => [
                'label' => '子ども用の椅子、食器類の提供',
                'score' => 1
            ],
            2 => [
                'label' => 'キッズ（未就学児向け）メニュー等の対応',
                'score' => 1
            ],
            3 => [
                'label' => '高齢者でも食べやすいメニューの提供',
                'score' => 1
            ],
            4 => [
                'label' => 'その他（申告制）',
                'score' => 1
            ]
        ]
    ],
    '7' => [
        'type'                  => TYPE_MULTI_MENU,
        'title'                 => '多様な食習慣への対応について、該当メニューと使用している食材、対応内容を入力してください。',
        'answer_title'          => 'メニューを登録',
        'menu_table_image'      => '',
        'store_image'           => '',
        'menu_max_item'         => 5,
        'ingredient_max_item'   => 2,
        'menus' => [
            1 => [
                'name'          => '',
                'image'         => '',
                'ingredients'   => [
                    1 => [
                        'name'              => '',
                        'effort_image'      => '',
                        'extra_information' => [
                            'appropriate_initiative' => [
                                'title' => '以下より該当する取り組みを選択してください。',
                                'name'  => 'appropriate_initiative',
                                'type'  => 'checkbox',
                                'options' => [
                                    1 => [
                                        'label' => 'ムスリムフレンドリーへの対応'. '<br>' .
                                                    'イスラム教の食文化「ハラール」に配慮したメニューを２品以上提供可能（豚（豚由来成分を含む）およびア' . '<br>' .
                                                    'ルコール（料理酒、みりん等）を使わない料理等）',
                                        'score' => 3
                                    ],
                                    2 => [
                                        'label' => 'ベジタリアンへの対応' . '<br>' .
                                                    '肉類および魚介類を使わない料理等を２品以上提供可能',
                                        'score' => 3
                                    ],
                                    3 => [
                                        'label' => 'アレルギーの原因となる食材の使用について説明できる表示や従業員マニュアルの整備',
                                        'score' => 1
                                    ],
                                    4 => [
                                        'label' => 'その他（申告制：アレルギー、グルテンフリーなどに配慮したメニューを１品以上提供可能できる）',
                                        'score' => 1
                                    ],
                                ]
                            ]
                        ]
                    ]
                ]
            ],
            2 => [
                'name'          => '',
                'image'         => '',
                'ingredients'   => [
                    1 => [
                        'name'              => '',
                        'effort_image'      => '',
                        'extra_information' => [
                            'appropriate_initiative' => [
                                'title' => '以下より該当する取り組みを選択してください。',
                                'name'  => 'appropriate_initiative',
                                'type'  => 'checkbox',
                                'options' => [
                                    1 => [
                                        'label' => 'ムスリムフレンドリーへの対応'. '<br>' .
                                                    'イスラム教の食文化「ハラール」に配慮したメニューを２品以上提供可能（豚（豚由来成分を含む）およびア' . '<br>' .
                                                    'ルコール（料理酒、みりん等）を使わない料理等）',
                                        'score' => 3
                                    ],
                                    2 => [
                                        'label' => 'ベジタリアンへの対応' . '<br>' .
                                                    '肉類および魚介類を使わない料理等を２品以上提供可能',
                                        'score' => 3
                                    ],
                                    3 => [
                                        'label' => 'アレルギーの原因となる食材の使用について説明できる表示や従業員マニュアルの整備',
                                        'score' => 1
                                    ],
                                    4 => [
                                        'label' => 'その他（申告制：アレルギー、グルテンフリーなどに配慮したメニューを１品以上提供可能できる）',
                                        'score' => 1
                                    ],
                                ]
                            ]
                        ]
                    ]
                ]
            ],
            3 => [
                'name'          => '',
                'image'         => '',
                'ingredients'   => [
                    1 => [
                        'name'              => '',
                        'effort_image'      => '',
                        'extra_information' => [
                            'appropriate_initiative' => [
                                'title' => '以下より該当する取り組みを選択してください。',
                                'name'  => 'appropriate_initiative',
                                'type'  => 'checkbox',
                                'options' => [
                                    1 => [
                                        'label' => 'ムスリムフレンドリーへの対応'. '<br>' .
                                                    'イスラム教の食文化「ハラール」に配慮したメニューを２品以上提供可能（豚（豚由来成分を含む）およびア' . '<br>' .
                                                    'ルコール（料理酒、みりん等）を使わない料理等）',
                                        'score' => 3
                                    ],
                                    2 => [
                                        'label' => 'ベジタリアンへの対応' . '<br>' .
                                                    '肉類および魚介類を使わない料理等を２品以上提供可能',
                                        'score' => 3
                                    ],
                                    3 => [
                                        'label' => 'アレルギーの原因となる食材の使用について説明できる表示や従業員マニュアルの整備',
                                        'score' => 1
                                    ],
                                    4 => [
                                        'label' => 'その他（申告制：アレルギー、グルテンフリーなどに配慮したメニューを１品以上提供可能できる）',
                                        'score' => 1
                                    ],
                                ]
                            ]
                        ]
                    ]
                ]
            ],
            4 => [
                'name'          => '',
                'image'         => '',
                'ingredients'   => [
                    1 => [
                        'name'              => '',
                        'effort_image'      => '',
                        'extra_information' => [
                            'appropriate_initiative' => [
                                'title' => '以下より該当する取り組みを選択してください。',
                                'name'  => 'appropriate_initiative',
                                'type'  => 'checkbox',
                                'options' => [
                                    1 => [
                                        'label' => 'ムスリムフレンドリーへの対応'. '<br>' .
                                                    'イスラム教の食文化「ハラール」に配慮したメニューを２品以上提供可能（豚（豚由来成分を含む）およびア' . '<br>' .
                                                    'ルコール（料理酒、みりん等）を使わない料理等）',
                                        'score' => 3
                                    ],
                                    2 => [
                                        'label' => 'ベジタリアンへの対応' . '<br>' .
                                                    '肉類および魚介類を使わない料理等を２品以上提供可能',
                                        'score' => 3
                                    ],
                                    3 => [
                                        'label' => 'アレルギーの原因となる食材の使用について説明できる表示や従業員マニュアルの整備',
                                        'score' => 1
                                    ],
                                    4 => [
                                        'label' => 'その他（申告制：アレルギー、グルテンフリーなどに配慮したメニューを１品以上提供可能できる）',
                                        'score' => 1
                                    ],
                                ]
                            ]
                        ]
                    ]
                ]
            ],
            5 => [
                'name'          => '',
                'image'         => '',
                'ingredients'   => [
                    1 => [
                        'name'              => '',
                        'effort_image'      => '',
                        'extra_information' => [
                            'appropriate_initiative' => [
                                'title' => '以下より該当する取り組みを選択してください。',
                                'name'  => 'appropriate_initiative',
                                'type'  => 'checkbox',
                                'options' => [
                                    1 => [
                                        'label' => 'ムスリムフレンドリーへの対応'. '<br>' .
                                                    'イスラム教の食文化「ハラール」に配慮したメニューを２品以上提供可能（豚（豚由来成分を含む）およびア' . '<br>' .
                                                    'ルコール（料理酒、みりん等）を使わない料理等）',
                                        'score' => 3
                                    ],
                                    2 => [
                                        'label' => 'ベジタリアンへの対応' . '<br>' .
                                                    '肉類および魚介類を使わない料理等を２品以上提供可能',
                                        'score' => 3
                                    ],
                                    3 => [
                                        'label' => 'アレルギーの原因となる食材の使用について説明できる表示や従業員マニュアルの整備',
                                        'score' => 1
                                    ],
                                    4 => [
                                        'label' => 'その他（申告制：アレルギー、グルテンフリーなどに配慮したメニューを１品以上提供可能できる）',
                                        'score' => 1
                                    ],
                                ]
                            ]
                        ]
                    ]
                ]
            ],
        ]
    ],
    '8' => [
        'type'              => TYPE_CHECKBOX,
        'title'             => '地域貢献活動について、以下より該当する取り組みについて選択してください。',
        'effort_detail'     => '',
        'effort_image'      => '',
        'options'   => [
            1 => [
                'label' => '地元地域との交流【申告制】',
                'score' => 2
            ],
            2 => [
                'label' => '地域の環境保全に配慮した取組の実施【申告制】',
                'score' => 2
            ],
            3 => [
                'label' => '子ども食堂・高齢者食堂等の活動への参画または開催【申告制】',
                'score' => 3
            ],
            4 => [
                'label' => '地域ボランティアへの参加【申告制】',
                'score' => 3
            ]
        ]
    ],
];

Configure::write('restaurantQuestionsScore', $restaurantQuestions);
