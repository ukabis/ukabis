<?php

return [
    'questions' => [
        '1' => [
            'group_question' => '生態',
            'title' => '環境負荷軽減等に向けた認証等について、以下から取り組んでいる項目を選択してください。',
            'allow_multi' => true,
            'has_file' => false,
            'options' => [
                [
                    'title' => '有機ＪＡＳ認証の取得',
                    'has_file' => true,
                    'upload_file_label' => '認定証添付',
                    'score' => 10
                ],
                [
                    'title' => 'JGAP認証等の取得',
                    'has_file' => true,
                    'upload_file_label' => '認定証添付',
                    'score' => 10
                ],
                [
                    'title' => '特別栽培農産物認定、エコファーマー認定の取得',
                    'has_file' => true,
                    'upload_file_label' => '認定証添付',
                    'score' => 5
                ],
                [
                    'title' => '環境保全型農業直接支払交付金事業で有機農業に取り組んでいる',
                    'has_file' => true,
                    'upload_file_label' => '交付決定通知添付',
                    'score' => 5
                ],
                [
                    'title' => '化学農薬を使用しない農業に取り組んでいる。',
                    'has_file' => false,
                    'score' => 5
                ],
                [
                    'title' => '取り組んでいない。',
                    'has_file' => false,
                    'score' => 0
                ]
            ]
        ],
        '2' => [
            'group_question' => '生態',
            'title' => '温室効果ガスの削減について、施設等の温度をチェック簿等で管理していますか？',
            'allow_multi' => false,
            'has_file' => false,
            'options' => [
                [
                    'title' => '管理している。',
                    'has_file' => false,
                    'score' => 1
                ],
                [
                    'title' => '管理していない。',
                    'has_file' => false,
                    'score' => 0
                ]
            ]
        ],
        '3' => [
            'group_question' => '生態',
            'title' => '施肥について、購入伝票の保管等適切な原材料調達を実施していますか？',
            'allow_multi' => false,
            'has_file' => false,
            'options' => [
                [
                    'title' => '実施している。',
                    'has_file' => false,
                    'score' => 1
                ],
                [
                    'title' => '実施していない。',
                    'has_file' => false,
                    'score' => 0
                ]
            ]
        ],
        '4' => [
            'group_question' => '生態',
            'title' => '循環型農業について、以下から取り組んでいる項目を選択してください。',
            'allow_multi' => true,
            'has_file' => false,
            'options' => [
                [
                    'title' => '残渣等の堆肥利用（未利用資源の活用）',
                    'has_file' => false,
                    'score' => 5
                ],
                [
                    'title' => '廃液を減らす取組をしている。',
                    'has_file' => false,
                    'score' => 5
                ],
                [
                    'title' => '地域内の堆肥を利用（耕畜連携）',
                    'has_file' => false,
                    'score' => 5
                ],
                [
                    'title' => '取り組んでいない。',
                    'has_file' => false,
                    'score' => 0
                ]
            ]
        ],
        '5' => [
            'group_question' => '生態',
            'title' => '農水省のCO2見える化シートを作成（脱炭素の取組を見える化）やカーボンクレジット制度（J-クレジット等）を活用など、脱炭素に取り組んでいますか？',
            'allow_multi' => false,
            'has_file' => true,
            'upload_file_label' => '計算シート添付',
            'options' => [
                [
                    'title' => '取り組んでいる',
                    'has_file' => false,
                    'score' => 10
                ],
                [
                    'title' => '取り組んでいない。',
                    'has_file' => false,
                    'score' => 0
                ]
            ]
        ],
        '6' => [
            'group_question' => '生態',
            'title' => '生物多様性や動物福祉、ＩＰＭの実践について、以下から取り組んでいる項目を選択してください。',
            'allow_multi' => true,
            'has_file' => false,
            'options' => [
                [
                    'title' => 'ＩＰＭ実践指標モデルを防除指針へ反映している。',
                    'has_file' => false,
                    'score' => 1
                ],
                [
                    'title' => 'アニマルウェルフェアのマニュアル策定',
                    'has_file' => false,
                    'score' => 1
                ]
            ]
        ],
        '7' => [
            'group_question' => '生態',
            'title' => '農業・農村の多面的機能の維持・発揮に関する活動について、中山間直接支払、多面的機能支払交付金事業に取り組んでいますか？',
            'allow_multi' => false,
            'has_file' => true,
            'upload_file_label' => '交付決定通知添付',
            'options' => [
                [
                    'title' => '取り組んでいる。',
                    'has_file' => false,
                    'score' => 5
                ],
                [
                    'title' => '取り組んでいない。',
                    'has_file' => false,
                    'score' => 0
                ]
            ]
        ],
        '8' => [
            'group_question' => '生態',
            'title' => '耕作放棄地の利用について、荒廃農地を利用して農業を実践していますか？',
            'allow_multi' => false,
            'has_file' => true,
            'upload_file_label' => '契約書添付',
            'options' => [
                [
                    'title' => '実践している。',
                    'has_file' => false,
                    'score' => 5
                ],
                [
                    'title' => '実践していない。',
                    'has_file' => false,
                    'score' => 0
                ]
            ]
        ],
        '9' => [
            'group_question' => '社会',
            'title' => '農福連携について、障害のある人の働く機会を創出し、雇用していますか？',
            'allow_multi' => false,
            'has_file' => true,
            'upload_file_label' => '契約書添付',
            'options' => [
                [
                    'title' => '実践している。',
                    'has_file' => false,
                    'score' => 10
                ],
                [
                    'title' => '実践していない。',
                    'has_file' => false,
                    'score' => 0
                ]
            ]
        ],
        '10' => [
            'group_question' => '社会',
            'title' => '従業員が働きがいを感じ、安心して働ける環境の整備について、以下から取り組んでいる項目を選択してください。',
            'allow_multi' => true,
            'has_file' => false,
            'options' => [
                [
                    'title' => '適正な就労契約（勤務時間等）',
                    'has_file' => false,
                    'score' => 1
                ],
                [
                    'title' => '労災保険等の加入',
                    'has_file' => false,
                    'score' => 5
                ],
                [
                    'title' => '危険性の高い作業（機械、高所、熱中症等）に関するマニュアル策定',
                    'has_file' => false,
                    'score' => 1
                ],
                [
                    'title' => 'トイレなど労働環境の整備',
                    'has_file' => false,
                    'score' => 1
                ],
                [
                    'title' => '従業員の研修参加を推進',
                    'has_file' => false,
                    'score' => 1
                ],
                [
                    'title' => '取り組んでいない。',
                    'has_file' => false,
                    'score' => 0
                ]
            ]
        ],
        '11' => [
            'group_question' => '社会',
            'title' => '法令を遵守した適正な生産について、以下から取り組んでいる項目を選択してください。',
            'allow_multi' => true,
            'has_file' => false,
            'options' => [
                [
                    'title' => '適正な燃料保管（鍵、漏洩防止、地元消防の確認等）',
                    'has_file' => false,
                    'score' => 1
                ],
                [
                    'title' => '適正な農薬保管（鍵、毒劇表示）',
                    'has_file' => false,
                    'score' => 1
                ],
                [
                    'title' => '施肥指針の策定',
                    'has_file' => false,
                    'score' => 1
                ],
                [
                    'title' => '女性の働きやすい職場作りの実践',
                    'has_file' => true,
                    'upload_file_label' => '家族経営協定等添付',
                    'score' => 1
                ],
                [
                    'title' => '取り組んでいない。',
                    'has_file' => false,
                    'score' => 0
                ]
            ]
        ],
        '12' => [
            'group_question' => '文化',
            'title' => '食品の衛生管理について、以下から取り組んでいる項目を選択してください。',
            'allow_multi' => true,
            'has_file' => false,
            'options' => [
                [
                    'title' => '集出荷貯蔵施設の衛生管理マニュアルの策定',
                    'has_file' => false,
                    'score' => 1
                ],
                [
                    'title' => '従業員にSDGs認証制度の取組を説明している。',
                    'has_file' => true,
                    'upload_file_label' => '認証書（本認証制度）添付',
                    'score' => 5
                ],
                [
                    'title' => '従業員に経営理念を明文化し、説明している。',
                    'has_file' => false,
                    'score' => 5
                ],
                [
                    'title' => '喫煙・飲食場所の指定',
                    'has_file' => false,
                    'score' => 1
                ],
                [
                    'title' => '全ての水源を把握し、安全性を確認している。',
                    'has_file' => false,
                    'score' => 1
                ],
                [
                    'title' => '完熟堆肥の利用',
                    'has_file' => false,
                    'score' => 5
                ],
                [
                    'title' => '取り組んでいない。',
                    'has_file' => false,
                    'score' => 0
                ]
            ]
        ],
        '13' => [
            'group_question' => '文化',
            'title' => '農業の未来・文化への貢献について、以下から取り組んでいる項目を選択してください。',
            'allow_multi' => true,
            'has_file' => false,
            'options' => [
                [
                    'title' => '後継者の育成',
                    'has_file' => false,
                    'score' => 5
                ],
                [
                    'title' => '伝統農法・技術の活用',
                    'has_file' => true,
                    'upload_file_label' => '世界遺産認定書添付',
                    'score' => 5
                ],
                [
                    'title' => '機能性表示食品、栄養機能食品等の機能面標記を実施している。',
                    'has_file' => true,
                    'upload_file_label' => '表事例添付',
                    'score' => 10
                ],
                [
                    'title' => '取り組んでいない。',
                    'has_file' => false,
                    'score' => 0
                ]
            ]
        ],
        '14' => [
            'group_question' => '経済',
            'title' => '消費者に信頼される農産物の出荷について、出荷販売記録の保管（出荷先、日付、数量）を実施していますか？',
            'allow_multi' => false,
            'has_file' => false,
            'options' => [
                [
                    'title' => '実施している。',
                    'has_file' => false,
                    'score' => 5
                ],
                [
                    'title' => '実施していない。',
                    'has_file' => false,
                    'score' => 0
                ]
            ]
        ],
        '15' => [
            'group_question' => '経済',
            'title' => '農業における食育・地域貢献について、以下から取り組んでいる項目を選択してください。',
            'allow_multi' => true,
            'has_file' => false,
            'options' => [
                [
                    'title' => '生産地域へ出荷している（フードマイレージ）',
                    'has_file' => false,
                    'score' => 5
                ],
                [
                    'title' => '食育活動（農業体験、学校等への出前講座）',
                    'has_file' => false,
                    'score' => 10
                ],
                [
                    'title' => 'ＢＣＰ策定（災害、病気、事故等）',
                    'has_file' => false,
                    'score' => 10
                ],
                [
                    'title' => '子ども食堂への提供、フードバンクへの提供',
                    'has_file' => false,
                    'score' => 5
                ],
                [
                    'title' => '取り組んでいない。',
                    'has_file' => false,
                    'score' => 0
                ]
            ]
        ],
        '16' => [
            'group_question' => '経済',
            'title' => '廃棄物の削減と適正な処理について、廃棄物処理マニュアルの策定に取り組んでいますか？',
            'allow_multi' => false,
            'has_file' => false,
            'options' => [
                [
                    'title' => '取り組んでいる。',
                    'has_file' => false,
                    'score' => 1
                ],
                [
                    'title' => '取り組んでいない。',
                    'has_file' => false,
                    'score' => 0
                ]
            ]
        ],
        '17' => [
            'group_question' => '説明責任',
            'title' => '農産物の生産履歴の整備について、以下から取り組んでいる項目を選択してください。',
            'allow_multi' => true,
            'has_file' => false,
            'options' => [
                [
                    'title' => '防除履歴の作成',
                    'has_file' => false,
                    'score' => 1
                ],
                [
                    'title' => '生産履歴の作成（２年は保存）',
                    'has_file' => false,
                    'score' => 5
                ],
                [
                    'title' => '市場関係者、飲食店等のニーズを把握し改善に努めている。',
                    'has_file' => false,
                    'score' => 5
                ],
                [
                    'title' => '内部検査マニュアルの策定',
                    'has_file' => false,
                    'score' => 1
                ],
                [
                    'title' => '取り組んでいない。',
                    'has_file' => false,
                    'score' => 0
                ]
            ]
        ],
        '18' => [
            'group_question' => '説明責任',
            'title' => '不具合が生じた時を想定した準備として、以下から取り組んでいる項目を選択してください。',
            'allow_multi' => true,
            'has_file' => false,
            'options' => [
                [
                    'title' => '生産履歴及び防除履歴が不適であった場合の対応マニュアルの策定',
                    'has_file' => false,
                    'score' => 1
                ],
                [
                    'title' => '残留農薬分析で基準値を超過時のマニュアル策定',
                    'has_file' => false,
                    'score' => 1
                ],
                [
                    'title' => '直接出荷先から当該出荷物の履歴等請求に対するマニュアルの策定',
                    'has_file' => false,
                    'score' => 1
                ],
                [
                    'title' => '取り組んでいない。',
                    'has_file' => false,
                    'score' => 0
                ]
            ]
        ],
        '19' => [
            'group_question' => '説明責任',
            'title' => '消費者等の問合せ先の明示について、以下から取り組んでいる項目を選択してください。',
            'allow_multi' => true,
            'has_file' => false,
            'options' => [
                [
                    'title' => '消費者からの問合せ対応マニュアルの策定',
                    'has_file' => false,
                    'score' => 1
                ],
                [
                    'title' => '専用ホームページへの登録',
                    'has_file' => false,
                    'score' => 1
                ],
                [
                    'title' => '取り組んでいない。',
                    'has_file' => false,
                    'score' => 0
                ]
            ]
        ],
        '20' => [
            'group_question' => null,
            'title' => '周辺住民への配慮について、農薬散布時等に事前のお知らせをしていますか？',
            'allow_multi' => false,
            'has_file' => false,
            'options' => [
                [
                    'title' => '実施している。',
                    'has_file' => false,
                    'score' => 5
                ],
                [
                    'title' => '実施していない。',
                    'has_file' => false,
                    'score' => 0
                ]
            ]
        ],
    ],

    'max_score_group' => [
        'group1' => [
            'questions' => ['1', '2', '3', '4', '5', '6', '7', '8'],
            'max_score' => 20
        ],
        'group2' => [
            'questions' => ['9', '10', '11'],
            'max_score' => 20
        ],
        'group3' => [
            'questions' => ['12', '13'],
            'max_score' => 20
        ],
        'group4' => [
            'questions' => ['14', '15', '16'],
            'max_score' => 20
        ],
        'group5' => [
            'questions' => ['17', '18', '19', '20'],
            'max_score' => 20
        ]
    ]
];
