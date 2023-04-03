<?php
/**
 * @var \App\View\AppView $this
 */
?>
<?php $this->assign('title', '飲食店 登録ページ');?>
<section class="content">
        <article class="wrap_contents">
        <?= $this->Flash->render() ?>
          <section class="name_sec name_id">
            <h2>ID申請画面</h2>
          </section>
          <section class="contents_sec id_registration">
            <div class="contents">
              <?= $this->Form->create($restaurant); ?>
              <div class="id_input">
                <?php
                    echo $this->Form->control('name', ['placeholder' => '氏名', 'label' => false, 'required' => false]);
                    echo $this->Form->control('email', ['placeholder' => 'メールアドレス', 'label' => false, 'required' => false, 'type' => 'text']);
                    echo $this->Form->control('email_confirm', ['placeholder' => 'メールアドレス（再入力）', 'label' => false, 'required' => false]);
                    echo $this->Form->control('password', ['placeholder' => 'パスワード', 'label' => false, 'required' => false]);
                    echo $this->Form->control('password_confirm', ['type' => 'password', 'placeholder' => 'パスワード（再入力）', 'label' => false, 'required' => false]);
                ?>
                <p class="id_authentication_btn">
                  	<?= $this->Form->button('新規登録'); ?>
                </p>
                <?= $this->Form->end(); ?>
                <section class="common_btn_sec">
                  <h3>既にアカウントをお持ちの方はこちら。</h3>
                  <?= $this->Html->link(
                        'ログイン',
                        $this->Url->build([
                            'prefix' => 'Restaurants',
                            'controller' => 'Auth',
                            'action' => 'login',
                        ], ['fullBase' => true]),
                        ['class' => 'login_btn']); ?>
                </section>
              </div>
            </div>
          </section>
        </article>
</section>

