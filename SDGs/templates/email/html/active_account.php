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
$appName = \Cake\Core\Configure::read('app_name');

echo 'このメッセージは'. $appName . 'サービス登録システムより自動送信されています。<br>
ーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーー<br>
<br>'
. $params->name . '様<br>
■認証コードは: '. $params->verification_code .'<br>
<br>
ーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーー<br>'
. EMAIL_SIGN .
'ーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーー';
