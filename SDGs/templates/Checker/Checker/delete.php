<?php

/**
 * @var \App\View\AppView $this
 */
?>
<?php $this->assign('title', '認証者 削除');?>
<article class="contents">
    <section class="title_sec">
        <h2>認証者 削除</h2>
    </section>

    <section class="alert_sec">
        <p>こちらのアカウントを削除しますか？</p>
    </section>
    <section class="list_sec remove_table">
        <table class="table-responsive">
            <tr>
                <th>ID</th>
                <th>氏名</th>
                <th>メールアドレス</th>
            </tr>
            <tr>
                <td><?= $checker->id ?></td>
                <td><?= $this->CustomHtml->renderTextWithMaxLength($checker->name) ?></td>
                <td><?= $this->CustomHtml->renderTextWithMaxLength($checker->email, 20) ?></td>
            </tr>
        </table>
    </section>

    <section class="btn_sec">
        <?= $this->Form->create($checker,
            [
                'type' => 'delete',
                'url' => $this->Url->build([
                    'prefix' => 'Checker',
                    'controller' => 'Checker',
                    'action' => 'delete',
                    'id' => $checker->id
                ], ['fullBase' => true])
            ]
        ) ?>
        <?= $this->Form->submit('削除する', ['class' => 'remove']) ?>
        <?= $this->Form->end(); ?>
        <p><button class="no backBtn">
            <?= $this->Html->link('一覧に戻る', $this->Url->build([
                'prefix' => 'Checker',
                'controller' => 'Checker',
                'action' => 'list',
            ], ['fullBase' => true])) ?>
        </button></p>
    </section>

</article>
