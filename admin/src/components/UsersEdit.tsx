import { SimpleForm, Edit, TextInput } from "react-admin";

interface PodcastCreateProps {
   basePath?: string;
}

export const UsersEdit = (props: PodcastCreateProps) => {
   return (
      <Edit title="Edit User" {...props}>
         <SimpleForm>
            <TextInput disabled source="id" />
            <TextInput source="username" />
            <TextInput source="fullname" />
            <TextInput source="email" />
            <TextInput source="password" />
            <TextInput source="lastlogin" />
            <TextInput source="firstlogin" />
            <TextInput source="conIP" />
            <TextInput source="birthday" />
         </SimpleForm>
      </Edit>
   );
};

export default UsersEdit;
