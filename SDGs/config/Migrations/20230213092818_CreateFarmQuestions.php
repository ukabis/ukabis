<?php
declare(strict_types=1);

use Migrations\AbstractMigration;

class CreateFarmQuestions extends AbstractMigration
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
        $table = $this->table('farm_questions', ['id' => false, 'primary_key' => ['id'], 'collation'=>'utf8mb4_unicode_ci']);

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

        for ($i = 1; $i <= TOTAL_FARM_QUESTIONS; $i++) {
            $table->addColumn(sprintf("question%d_score", $i), 'json', [
                'null' => true,
                'default' => null,
            ]);
        }

        $table->addColumn('total_score', 'integer', [
            'limit' => 11,
            'null' => true,
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
