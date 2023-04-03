<?php
/**
 * @var \App\View\AppView $this
 */
?>
<!DOCTYPE html>
<html lang="ja">
<head>
    <meta charset="utf-8">
</head>
<body>
    <div class="container">
        <div class="logo">
            <img src="images/certification/logo.png" alt="" width="75%">
        </div>
        <div class="office_info">
            <table>
                <tr>
                    <th><p>会社名</p></th>
                    <td><p><?= h($office_name) ?></p></td>
                </tr>
                <tr>
                    <th><p>代表者名（個人事業主）</p></th>
                    <td ><p><?= h($representative_name) ?></p></td>
                </tr>
            </table>
        </div>
        <div class="assignment">
            <p>
                ふじのくに SDGs 認定証の申請があった下記について、 <br>
                審査の結果、認定基準に適合することを確認できましたので <br>
                これを認証します。
            </p>
        </div>

        <div class="result">
            <div class="result_title">
                <p>記</p>
            </div>
            <div class="result_content">
                <div class="col12">
                    <div class="col6">
                        <div class="scoring">
                            <table>
                                <tr>
                                    <th><p style="margin-right: 70px;">総合得点</p></th>
                                    <td>
                                        <p>
                                            <span style="font-size: 24px; font-weight: bold; margin-right: -5px;">
                                                <?= h($total_score) ?>
                                            </span>
                                            点
                                        </p>
                                    </td>
                                </tr>
                            </table>

                            <hr style="border: 1px solid #9c9999;">
                            <hr style="border:none; border-top: 1px solid #000000; color: #000000; margin-top: -5px;">

                            <div class="rank">
                                <p><?= h($rank_label) ?>認証</p>
                            </div>

                            <hr style="border:none; border-top: 1px solid #000000; color: #000000; margin-bottom: -5px;">
                            <hr style="border: 1px solid #9c9999;">
                        </div>
                    </div>
                    <div class="col6">
                        <div class="mark">
                            <img src="<?= h($mark_image) ?>" alt="" width="90%" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="business_info">
            <p>事業者名：　<?= h($business_name) ?></p>
            <p>事業者住所：<?= h($business_address) ?></p>
            <p>認定年月日：<?= h($certification_date) ?></p>
        </div>

        <div class="signal">
            <p>静岡県知事　川勝 平太</p>
        </div>
    </div>
</body>
</html>
