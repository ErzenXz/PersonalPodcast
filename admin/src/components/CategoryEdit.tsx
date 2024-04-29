import { SimpleForm, Edit, TextInput } from "react-admin";

interface CategoryCreateProps {
   basePath?: string;
}

export const CategoryEdit = (props: CategoryCreateProps) => {
   return (
      <Edit title="Edit Category" {...props}>
         <SimpleForm>
            <TextInput disabled source="id" />
            <TextInput source="name" />
         </SimpleForm>
      </Edit>
   );
};

export default CategoryEdit;
