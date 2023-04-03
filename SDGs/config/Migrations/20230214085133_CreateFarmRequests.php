<?php
declare(strict_types=1);

use Migrations\AbstractMigration;

class CreateFarmRequests extends AbstractMigration
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
        $table = $this->table('farm_requests', ['id' => false, 'primary_key' => ['id'], 'collation'=>'utf8mb4_unicode_ci']);

        $table->addColumn('id', 'biginteger', [
            'autoIncrement' => true,
            'limit' => 20
        ]);

        $table->addColumn('office_id', 'biginteger', [
            'limit' => 20,
            'null' => false,
        ]);

        $table->addColumn('food', 'string', [
            'limit' => 50,
            'null' => false,
        ]);

        $table->addColumn('comment', 'string', [
            'limit' => 50,
            'null' => false,
        ]);

        $table->addColumn('created_at', 'timestamp', [
            'default' => 'CURRENT_TIMESTAMP',
            'null' => false,
        ]);

        $table->addColumn('modified_at', 'timestamp', [
            'default' => 'CURRENT_TIMESTAMP',
            'null' => false,
        ]);

        $table->addPrimaryKey("id");

        $table->create();
    }
}
