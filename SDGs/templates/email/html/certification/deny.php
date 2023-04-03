<?php

/**
 * CakePHP(tm) : Rapid Development Framework (https://cakephp.org)
 * Copyright (c) Cake Software Foundation, Inc. (https://cakefoundation.org)
 *
 * Licensed under The MIT License
 * For full copyright and license information, please see the LICENSE.txt
 * Redistributions of files must retain the above copyright notice.
 *
 * @copyright     Copyright (c) Cake Software Foundation, Inc. (https://cakefoundation.org)
 * @link          https://cakephp.org CakePHP(tm) Project
 * @since         0.10.0
 * @license       https://opensource.org/licenses/mit-license.php MIT License
 * @var \App\View\AppView $this
 * @var object $params
 */

echo '事業者名:' . $params->business_name . '<br>
この度は、ふじのくにSDGs認証への申請、誠にありがとうございます。<br>
<br>
厳正な審査の元、申請頂きました内容を確認した結果<br>
事業者名 様の申請内容については以下の結果となりました事<br>
ご報告いたします。<br>
<br>
ーーーーーーーーー<br>
総合得点 : ' . $params->total_score . ' 点<br>
認証ステタス : 不合格<br>
認証ランク : ' . $params->certificate_rank_label . '<br>
ーーーーーーーーー<br>
<br>
残念ではございますが、申請いただいた内容を確認した結果<br>
不合格とさせて頂きました。<br>
再度、申請内容のご確認およびご対応をいただき、<br>
改めて申請していただけますようお願い致します。<br>
ご質問等がございましたら、<br>
別途以下事務局までお問い合わせください。<br>
<br>
ーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーー<br>'
    . EMAIL_SIGN .
    'ーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーー';
