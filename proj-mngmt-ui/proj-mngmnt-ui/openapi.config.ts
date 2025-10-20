import { GeneratorConfig } from 'ng-openapi';

const config: GeneratorConfig = {
  input: './src/assets/openapi.schema.json',
  output: './src/client',
  options: {
    dateType: 'Date',
    enumStyle: 'enum',
  },
};

export default config;
