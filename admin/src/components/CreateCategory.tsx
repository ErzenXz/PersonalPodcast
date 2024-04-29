import { SimpleForm, Create, TextInput } from "react-admin";

interface PodcastCreateProps {
   basePath?: string;
}

export const CreateCategory = (props: PodcastCreateProps) => {
   return (
      <Create title="Create a Episode" {...props}>
         <SimpleForm>
            <TextInput source="name" />
         </SimpleForm>
      </Create>
   );
};

export default CreateCategory;
