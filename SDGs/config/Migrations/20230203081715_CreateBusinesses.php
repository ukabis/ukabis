<?php
declare(strict_types=1);

use Migrations\AbstractMigration;

class CreateBusinesses extends AbstractMigration
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
        $table = $this->table('businesses', ['id' => false, 'primary_key' => ['id'], 'collation'=>'utf8mb4_unicode_ci']);

        $table->addColumn('id', 'biginteger', [
            'autoIncrement' => true,
            'limit' => 20
        ]);
        $table->addColumn('name', 'string', [
            'limit' => 255,
            'null' => false,
        ]);
        $table->addColumn('name_kana', 'string', [
            'limit' => 255,
            'null' => false,
        ]);
        $table->addColumn('zip_code', 'string', [
            'limit' => 20,
            'null' => false,
        ]);
        $table->addColumn('email', 'string', [
            'limit' => 255,
            'null' => false,
        ]);
        $table->addColumn('prefecture', 'string', [
            'limit' => 255,
            'null' => false,
        ]);
        $table->addColumn('city', 'string', [
            'limit' => 255,
            'null' => false,
        ]);
        $table->addColumn('street', 'string', [
            'limit' => 255,
            'null' => true,
        ]);
        $table->addColumn('representative_name', 'string', [
            'limit' => 255,
            'null' => true,
        ]);
        $table->addColumn('tel', 'string', [
            'limit' => 20,
            'null' => true,
        ]);
        $table->addColumn('fax', 'string', [
            'limit' => 20,
            'null' => true,
        ]);
        $table->addColumn('homepage_url', 'string', [
            'limit' => 255,
            'null' => true,
        ]);
        $table->addColumn('created_at', 'timestamp', [
            'default' => 'CURRENT_TIMESTAMP',
            'null' => false,
        ]);
        $table->addColumn('modified_at', 'timestamp', [
            'default' => 'CURRENT_TIMESTAMP',
            'null' => false,
        ]);

        $table->addPrimaryKey("id")
            ->addIndex('email', ['unique' => true]);
        $table->create();
    }
}
