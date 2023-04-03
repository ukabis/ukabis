<?php
declare(strict_types=1);

use Migrations\AbstractMigration;

class CreateRestaurantQuestions extends AbstractMigration
{
    /**
     * Change Method.
     *
     * More information on this method is available here:
     * https://book.cakephp.org/phinx/0/en/migrations.html#the-change-method
     * @return void
     */
    public function change(): void
    {
        $table = $this->table('restaurant_questions', ['id' => false, 'primary_key' => ['id']]);

        $table->addColumn('id', 'biginteger', [
            'autoIncrement' => true,
            'limit' => 20
        ]);

        $table->addColumn('office_id', 'biginteger', [
            'limit' => 20,
            'null' => false,
        ]);

        $table->addColumn('answer_questions', 'longtext', [
            'null' => false,
        ]);

        $table->addColumn('status', 'integer', [
            'limit' => 2,
            'null' => false,
            'default' => 0
        ]);

        for ($i = 1; $i <= 10; $i++) {
            $table->addColumn(sprintf("question_1_menu_%d_score", $i), 'json', [
                'limit' => 11,
                'null' => true
            ]);
        }

        $table->addColumn("question_2_score", 'json', [
            'limit' => 11,
            'null' => true
        ]);

        $table->addColumn("question_3_score", 'json', [
            'limit' => 11,
            'null' => true
        ]);

        $table->addColumn("question_4_score", 'json', [
            'limit' => 11,
            'null' => true
        ]);

        $table->addColumn("question_5_score", 'json', [
            'limit' => 11,
            'null' => true
        ]);

        $table->addColumn("question_6_score", 'json', [
            'limit' => 11,
            'null' => true
        ]);

        for ($i = 1; $i <= 5; $i++) {
            $table->addColumn(sprintf("question_7_menu_%d_score", $i), 'json', [
                'limit' => 11,
                'null' => true,
            ]);
        }

        $table->addColumn("question_8_score", 'json', [
            'limit' => 11,
            'null' => true
        ]);

        $table->addColumn('total_score', 'float', [
            'limit' => 11,
            'null' => false,
            'default' => 0
        ]);

        $table->addColumn('created_at', 'timestamp', [
            'default' => 'CURRENT_TIMESTAMP',
            'null' => false,
        ]);

        $table->addColumn('modified_at', 'timestamp', [
            'default' => 'CURRENT_TIMESTAMP',
            'null' => false,
        ]);

        $table->create();
    }
}
