class CreateIndexModels < ActiveRecord::Migration[5.1]
  def change
    create_table :index_models do |t|
      t.string :metadata
      t.string :testButtonsDisplay

      t.timestamps
    end
  end
end
